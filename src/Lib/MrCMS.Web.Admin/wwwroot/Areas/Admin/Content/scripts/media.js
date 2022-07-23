export function fileTools(options) {
    const settings = $.extend({
        folderClass: ".folder",
        activeFolderClass: '.active',
        fileClass: ".file",
        fileToolsContainer: "#file-tools",
        deleteFilesBtn: "#delete-files",
        pasteFilesBtn: "#paste-files",
        clearFilesBtn: "#clear-files",
        cutFilesBtn: "#cut-files",

        filesStorageKey: "files",
        cutFilesStorageKey: "cut-files",

        fileCutClass: ".opacity-50",
    }, options);
    let self;
    return {
        init: function () {
            self = this;
            $(document).on('initialize-plugins', function () {

            });

            //enable single click on the folder
            $(document).on('click', settings.folderClass, function (e) {
                e.preventDefault();
                $(this).toggleClass(settings.activeFolderClass.replace('.', ''));
                self.selectedStop();
            })

            //enbales double click on folder
            $(document).on('dblclick', settings.folderClass, function () {
                location.href = $(this).data('url');
            });

            //enable single click on the files
            $(document).on('click', settings.fileClass, function () {
                self.selectedStop();
            })


            $(document).on('click', settings.clearFilesBtn, function () {
                self.clearSelectedFiles();
                self.clearCutStyles();
                self.setSelectedFileData('');
                self.setCutFileData('');
                self.disableCut();
                self.disableDelete();
                self.disablePaste();
            });

            $(document).on('click', settings.cutFilesBtn, function (e) {
                e.preventDefault();
                self.clearCutStyles();
                self.copyCutFilesToStorage();
            });

            $(document).on('click', settings.pasteFilesBtn, function (e) {
                e.preventDefault();
                self.paste();
            });

            $(document).on('click', settings.deleteFilesBtn, function (e) {
                e.preventDefault();
                swal({
                    title: "Are you sure?",
                    text: "Your will not be able to recover this!",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Yes, delete it!",
                    closeOnConfirm: true
                }, function () {
                    self.deleteFiles();
                });
            });

            self.setSelectedFileData('');
            self.setSelectedFiles();
            return self;
        },
        deleteFiles: function () {
            const filesAndFolders = self.getSelectedFileData().split(',');
            let files = filesAndFolders.filter(isFileId).join(',');
            files = replaceAll("file-", "", files);
            let folders = filesAndFolders.filter(isFolderId).join(',');
            folders = replaceAll("folder-", "", folders);

            $.ajax({
                type: "POST",
                url: "/Admin/MediaCategory/DeleteFilesAndFolders",
                data: {
                    files: files,
                    folders: folders
                },
                success: function (data) {
                    self.clearCutStyles();
                    self.clearSelectedFiles();
                    self.setSelectedFileData('');
                    self.setCutFileData('');
                    if (data.message != '')
                        alert(data.message);
                    self.setButtonState();
                    $(document).trigger('update-area', 'media-directory');
                }
            });
        },
        paste: function () {
            const filesAndFolders = self.getCutFileData().split(',');
            let files = filesAndFolders.filter(isFileId).join(',');
            files = replaceAll("file-", "", files);
            let folders = filesAndFolders.filter(isFolderId).join(',');
            folders = replaceAll("folder-", "", folders);
            $.ajax({
                type: "POST",
                url: "/Admin/MediaCategory/MoveFilesAndFolders",
                data: {
                    folderId: $("#FolderId").val(),
                    files: files,
                    folders: folders
                },
                success: function (data) {
                    self.clearCutStyles();
                    self.clearSelectedFiles();
                    self.setSelectedFileData('');
                    self.setCutFileData('');
                    if (data.message != '')
                        alert(data.message);
                    self.setButtonState();
                    $(document).trigger('update-area', 'media-directory');
                }
            });
        },
        copyCutFilesToStorage: function () {
            self.disableCut();
            self.enablePaste();
            self.setCutFileData(self.getSelectedFileData());
            self.setOpacityOnCutFiles(self.getCutFileData());
        },
        setOpacityOnCutFiles: function (files) {
            let fileIds = files.split(',');
            $.each(fileIds, function (_, id) {
                const selector = "[data-id='" + id + "']";
                const fileCutClass = settings.fileCutClass.replace('.', '');
                if (isFileId(id))
                    $(selector).parent().parent().addClass(fileCutClass);

                if (isFolderId(id))
                    $(selector).addClass(fileCutClass)
            });
        },
        selectedStop: function () {
            const data = $(settings.folderClass + settings.activeFolderClass + "," + settings.fileClass + ">input:checked")
                .map(function () {
                    return $(this).data('id');
                }).get().join(",");
            self.setSelectedFileData(data);
            self.setButtonState();
        },
        setSelectedFiles: function () {
            const filesAndFolders = self.getSelectedFileData();
            if (typeof filesAndFolders != 'undefined' && filesAndFolders !== '' && filesAndFolders != null) {
                const filesAndFoldersIds = filesAndFolders.split(',');
                $.each(filesAndFoldersIds, function (_, id) {
                    const selector = "[data-id='" + id + "']";
                    if (isFolderId(id))
                        $(selector).addClass(settings.activeFolderClass.replace('.', ''));

                    if (isFileId(id))
                        $(selector + ">input[type=checkbox]").prop("checked", true);
                });
            }
            self.setOpacityOnCutFiles(self.getCutFileData());
            self.setButtonState();
        },
        setButtonState: function () {
            self.disableCut();
            self.disablePaste();
            self.disableDelete();
            if (self.getSelectedFileData() !== '') {
                self.enableCut();
                self.enableDelete();
            } else if (self.getSelectedFileData() !== '' && self.getCutFileData() === '') {
                self.enableCut();
            }
            if (self.getCutFileData() !== '') {
                self.enablePaste();
            }
        },
        clearSelectedFiles: function () {
            $(settings.fileClass + ">input:checked").prop("checked", false);
            $(settings.folderClass).removeClass(settings.activeFolderClass.replace(".", ""));
        },
        clearCutStyles: function () {
            $(settings.fileCutClass).removeClass(settings.fileCutClass.replace('.', ''))
        },
        getSelectedFileData: function () {
            return store.get(settings.filesStorageKey);
        },
        getCutFileData: function () {
            return store.get(settings.cutFilesStorageKey);
        },
        setSelectedFileData: function (data) {
            store.set(settings.filesStorageKey, data);
            self.setButtonState();
        },
        setCutFileData: function (data) {
            store.set(settings.cutFilesStorageKey, data);
            self.setButtonState();
        },
        enableCut: function () {
            $(settings.cutFilesBtn).removeAttr("disabled");
        },
        disableCut: function () {
            $(settings.cutFilesBtn).attr("disabled", "disabled");
        },
        enableDelete: function () {
            $(settings.deleteFilesBtn).removeAttr("disabled");
        },
        disableDelete: function () {
            $(settings.deleteFilesBtn).attr("disabled", "disabled");
        },
        enablePaste: function () {
            $(settings.pasteFilesBtn).removeAttr("disabled");
        },
        disablePaste: function () {
            $(settings.pasteFilesBtn).attr("disabled", "disabled");
        }
    };
}

// (function () {
//     var fileTools = new FileTools().init();
// })();

function isFileId(element) {
    return element.indexOf("file") > -1;
}

function isFolderId(element) {
    return element.indexOf("folder") > -1;
}

function replaceAll(find, replace, str) {
    return str.replace(new RegExp(find, 'g'), replace);
}
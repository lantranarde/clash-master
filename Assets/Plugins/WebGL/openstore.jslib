mergeInto(LibraryManager.library, {
    OpenStore: function (urlPtr) {
        var url = UTF8ToString(urlPtr);
        if (!url || url === "") return;
        window.open(url, "_blank");
    }
});

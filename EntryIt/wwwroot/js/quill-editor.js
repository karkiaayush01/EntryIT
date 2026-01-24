let quill;

window.initQuill = (editorId) => {
    quill = new Quill(`#${editorId}`, {
        theme: "snow",
        modules: {
            toolbar: [
                ["bold", "italic", "underline"],
                [{ list: "ordered" }, { list: "bullet" }],
                [{ header: [1, 2, false] }],
                [{ 'color': [] }, { 'background': [] }],
                ["clean"]
            ]
        }
    });
};

window.quillInterop = {
    registerOnChange: function (dotNetRef) {
        if (!quill) {
            console.error("Quill not initialized");
            return;
        }

        quill.on("text-change", function () {
            dotNetRef.invokeMethodAsync("OnEditorChanged");
        });
    }
};

window.getQuillHtml = () => {
    return quill.root.innerHTML;
};

// try setting quill html until complete
window.setQuillHtml = (html) => {
    const trySet = () => {
        if (quill && quill.root) {
            quill.root.innerHTML = html ?? "";
        } else {
            // Quill not ready yet → retry
            setTimeout(trySet, 50);
        }
    };

    trySet();
};

window.getQuillText = () => {
    return quill.getText().trim();
};
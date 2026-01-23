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

window.setQuillHtml = (html) => {
    quill.root.innerHTML = html;
};

window.getQuillText = () => {
    return quill.getText().trim();
};
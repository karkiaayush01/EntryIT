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

window.getQuillHtml = () => {
    return quill.root.innerHTMl;
};

window.setQuillHtml = (html) => {
    quill.root.innerHTMl = html;
};
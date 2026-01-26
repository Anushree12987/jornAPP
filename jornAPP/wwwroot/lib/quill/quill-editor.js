window.quillEditors = {};

window.initQuill = function (editorId) {
    const selector = "#" + editorId;

    const quill = new Quill(selector, {
        theme: "snow",
        modules: {
            toolbar: "#contentToolbar"
        }
    });

    window.quillEditors[editorId] = quill;
};

window.getQuillHtml = function (editorId) {
    const editor = window.quillEditors[editorId];
    if (editor) return editor.root.innerHTML;
    return "";
};

console.log("quill-editor.js loaded correctly");

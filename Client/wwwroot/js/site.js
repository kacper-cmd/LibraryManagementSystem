﻿//export function setTheme(themeName) {
//    //add a new css link
//    let newLink = document.createElement("link");
//    newLink.setAttribute("id", "theme");
//    newLink.setAttribute("rel", "stylesheet");
//    newLink.setAttribute("type", "text/css");
//    newLink.setAttribute("href", `DarkMode.css`);
//    //remove the theme from the head tag
//    let head = document.getElementsByTagName("head")[0];
//    head.querySelector("#theme").remove();
//    //adding newLink in the head
//    head.appendChild(newLink);
//}
export function setTheme(themeName) {
    //remove the theme from the head tag
    let head = document.getElementsByTagName("head")[0];
    let d = head.querySelector("#theme");
    if (d) {
        d.remove();
    }
    if (themeName === "dark") {
        let newLink = document.createElement("link");
        newLink.setAttribute("id", "theme");
        newLink.setAttribute("rel", "stylesheet");
        newLink.setAttribute("type", "text/css");
        newLink.setAttribute("href", `DarkMode.css`);
        head.appendChild(newLink);
    }

}
export function downloadFile(mimeType, base64String, fileName) {

    var fileDataUrl = "data:" + mimeType + ";base64," + base64String;
    fetch(fileDataUrl)
        .then(response => response.blob())
        .then(blob => {

            //create a link
            var link = window.document.createElement("a");
            link.href = window.URL.createObjectURL(blob, { type: mimeType });
            link.download = fileName;

            //add, click and remove
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        });
}
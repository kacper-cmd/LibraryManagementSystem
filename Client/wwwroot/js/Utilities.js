﻿window.focusById =  function (elementId) {
    var element = document.getElementById(elementId);
    if (element) {
        element.focus();
    }
}

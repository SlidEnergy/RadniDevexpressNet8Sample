//Cliboard begin
function writeText(text) {
    navigator.clipboard.writeText(text);
}
//Cliboard end

function dragSplitter(element, first, second, direction) {
    var md;

    element.onmousedown = onMouseDown;

    function onMouseDown(e) {
        md = {
            e,
            offsetLeft: element.offsetLeft,
            offsetTop: element.offsetTop,
            firstWidth: first.offsetWidth,
            secondWidth: second.offsetWidth,
            firstHeight: first.offsetHeight,
            secondHeight: second.offsetHeight
        };

        document.onmousemove = onMouseMove;
        document.onmouseup = () => {
            document.onmousemove = document.onmouseup = null;
        }
    }

    function onMouseMove(e) {
        var delta = {
            x: e.clientX - md.e.clientX,
            y: e.clientY - md.e.clientY
        };

        if (direction === "H") {
            delta.x = Math.min(Math.max(delta.x, -md.firstWidth),
                md.secondWidth);

            element.style.left = md.offsetLeft + delta.x + "px";
            first.style.width = (md.firstWidth + delta.x) + "px";
            second.style.width = (md.secondWidth - delta.x) + "px";
        }

        if (direction === "V") {
            delta.y = Math.min(Math.max(delta.y, -md.firstHeight),
                md.secondHeight);

            element.style.top = md.offsetTop + delta.y + "px";
            first.style.height = (md.firstHeight + delta.y) + "px";
            second.style.height = (md.secondHeight - delta.y) + "px";
        }
        e.preventDefault();
        e.stopPropagation();
    }
}

function allowResize(ele, resizer) {
    // Query the element
    //const ele = document.getElementById('resizeMe');

    // The current position of mouse
    let x = 0;
    let y = 0;

    // The dimension of the element
    let w = 0;
    let h = 0;

    // Handle the mousedown event
    // that's triggered when user drags the resizer
    const mouseDownHandler = function (e) {
        // Get the current mouse position
        x = e.clientX;
        y = e.clientY;

        // Calculate the dimension of element
        const styles = window.getComputedStyle(ele);
        w = parseInt(styles.width, 10);
        h = parseInt(styles.height, 10);

        // Attach the listeners to `document`
        ele.style.transition = "none";
        document.addEventListener('mousemove', mouseMoveHandler);
        document.addEventListener('mouseup', mouseUpHandler);
    };

    const mouseMoveHandler = function (e) {
        // How far the mouse has been moved
        const dx = e.clientX - x;
        const dy = e.clientY - y;

        // Adjust the dimension of element
        ele.style.width = `${w - dx}px`;
        //ele.style.height = `${h - dy}px`;

        e.preventDefault();
        e.stopPropagation();    };

    const mouseUpHandler = function () {
        // Remove the handlers of `mousemove` and `mouseup`
        ele.style.transition = "width 650ms";
        document.removeEventListener('mousemove', mouseMoveHandler);
        document.removeEventListener('mouseup', mouseUpHandler);
    };

    resizer.addEventListener('mousedown', mouseDownHandler);
}

function showToast(elem, duration) {
    if (elem != null) {
        if (!elem.classList.contains('show')) {
            setTimeout(() => {
                elem.classList.add('show');
            }, 100);
        }

        setTimeout(() => {
            elem.classList.remove('show');
        }, (duration * 1000));
    }
}

function focusEditor(selector) {
    setTimeout(function () {
        let editor = $(selector)[0];
        
        editor && editor.focus();
    }, 500);
}

function setText(selector, text) {
    setTimeout(function () {
        var items = $(selector);

        if (items && items.length)
            items[0].value = text;
    }, 0);
}

function openBlankUrl(url) {
    window.open(url, "_blank");
}

function childOfById(element, id) {
    if (!id)
        return false;

    while (true) {
        if (!element)
            return false;

        if (element.id == id)
            return true;

        if (element.tagName == "BODY")
            return false;

        element = element.parentElement;
    }
}

function subscribeToTransitionEnd(dotNetReference, element) {
    if (element == null)
        return;

    element.ontransitionend = (e) => {
        if (e.srcElement.id === element.id)
            dotNetReference.invokeMethodAsync('OnTransitionEnd').then(nData => {});
    };
}

//Scroll begin
function scrollToView(elementId) {
    document.getElementById(elementId).scrollIntoView({ behavior: 'smooth' });
}

function scrollToTop(elementId) {

    const container = document.getElementById(elementId);

    container.scrollTo({
        top: 0,
        left: 0,
        behavior: 'smooth'
    });
}

function watchScroll(contId, tabs, dotNetHelper, methodName) {
    const elements = tabs.split(",");

    const container = document.getElementById(contId);

    container.addEventListener("scroll", function () {
        for (var i = 0; i <= elements.length - 1; i++) {
            var ele = document.getElementById(elements[i]);
            var position = ele.getBoundingClientRect();
            if (position.top >= 0 && position.bottom <= container.offsetHeight) {
                dotNetHelper.invokeMethod(methodName, elements[i]);
                return;
            }
        }
    });
}

//Scroll end

function setCustomDropDownWidth(id) {
    let input = document.getElementById(id);

    if (!input)
        return;

    let inputWidth = input.offsetWidth;

    let dropDown = document.getElementById(id + 'DropDown');

    if (!dropDown && !dropDown.parentElement)
        return;

    let dropDownWidth = dropDown.parentElement.offsetWidth || (dropDown.parentElement.__width && dropDown.parentElement.__width.replace && dropDown.parentElement.__width.replace('px', '')) || 0;

    if (inputWidth > dropDownWidth) {
        document.getElementById(id + 'DropDown').style.width = inputWidth + 'px';
        document.getElementById(id + 'DropDown').parentElement.style.width = inputWidth + 'px';
    }
}

function clearCustomDropDownWidth(id) {
    document.getElementById(id + 'DropDown').style.width = '';
}

function addClass(selector, className) {
    if (!className)
        return;

    var element = document.querySelector(selector);

    if (element) {
        var isList = className.includes(" ");

        if (isList) {
            var list = className.split(" ");

            list.forEach(x => element.classList.remove(x));
        } else
            element.classList.add(className);
    }
}

function removeClass(selector, className) {
    if (!className)
        return;

    var element = document.querySelector(selector);

    if (element) {
        var isList = className.includes(" ");

        if (isList) {
            var list = className.split(" ");

            list.forEach(x => element.classList.remove(x));
        } else
            element.classList.remove(className);
    }
}

export let utils = {
    clipboard: {
        writeText,
    },
    splitter: {
        dragSplitter
    },
    allowResize,
    showToast,
    focusEditor,
    setText,
    openBlankUrl,
    childOfById,
    subscribeToTransitionEnd,
    scrollToView,
    scrollToTop,
    watchScroll,
    setCustomDropDownWidth,
    clearCustomDropDownWidth,
    addClass,
    removeClass
};



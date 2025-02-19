let canvas = null;
let canvasRect = null;
let connections = null;
let connectionsRect = null;
let currentDragStep = null;
let currentDragResizer = null;
let currentDragPath = null;
let currentDragFrom = null;
let currentDragTo = null;
let tabs = null;

var stepsList;
var listenersList;
var triggersList;
var workflowsList;

function getSteps() {
    fetch('/api/steps')
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Fetch request failed');
            }
        }).then(data => {
            stepsList = data;
            getListeners();
        });
}

function getListeners() {
    fetch('/api/listeners')
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Fetch request failed');
            }
        }).then(data => {
            listenersList = data;
            getTriggers();
        });
}

function getTriggers() {
    fetch('/api/triggers')
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Fetch request failed');
            }
        }).then(data => {
            triggersList = data;
            getWorkflows();
        });
}

function getWorkflows() {
    fetch('/api/workflows')
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Fetch request failed');
            }
        }).then(data => {
            workflowsList = data;
            createTabs();
        });
}

function createTabs() {
    Object.entries(workflowsList).forEach(([key, _]) => {
        const tab = document.createElement('div');
        tab.setAttribute('class', 'tab');
        tab.setAttribute('data-tab', key);
        tab.innerHTML = key;
        tab.addEventListener("click", () => {
            changeTab(tab);
        });
        if (!tabs[0])
            changeTab(tab);
        document.getElementById('tabs').appendChild(tab);
    });
}

function changeTab(tab) {
    tabs.forEach(t => t.classList.remove("active"));
    tab.classList.add("active");
    workflowsList[tab.getAttribute("data-tab")].triggers.forEach((trigger) => {
        createTrigger(trigger);
        createSteps(trigger.steps);
        createRelations(trigger.relations);
    });
}

function createResizer() {
    const resizer = document.createElement('div');
    resizer.setAttribute('class', 'resizer');
    return resizer;
}

function createTrigger(trigger) {
    createElement(trigger, 'trigger', triggersList);
}

function createSteps(steps) {
    steps.forEach((step) => {
        createStep(step);
    });
}

function createStep(step) {
    createElement(step, 'step', stepsList);
}

function createElement(element, elementType, elementsList) {
    let currentElement = elementsList.filter(item => { return item.code === element.code })[0];
    const divElement = document.createElement('div');
    divElement.setAttribute('class', elementType);
    divElement.setAttribute('style', `top:${element.x}px; left:${element.y}px; height:${element.height}px; width:${element.width}px`);
    divElement.dataset.id = element.id;
    const divElementTitle = document.createElement('div');
    divElementTitle.setAttribute('class', `${elementType}title`);
    divElementTitle.innerHTML = currentElement.name;

    const divParameters = document.createElement('div');
    divParameters.setAttribute('class', 'parameters');
    var gridRowInputs = 1;
    currentElement.inputs.forEach((input) => {
        createInputParameter(element, elementType, input, divParameters, gridRowInputs);
        gridRowInputs++;
    });
    var gridRowOutputs = 1;
    currentElement.outputs.forEach((output) => {
        createOutputParameter(element, elementType, output, divParameters, gridRowOutputs);
        gridRowOutputs++;
    });

    const divConstants = document.createElement('div');
    divConstants.setAttribute('class', 'constants');
    var gridRowConstants = 1;
    currentElement.constants.forEach((constant) => {
        createConstantParameter(element, elementType, currentElement, constant, divConstants, gridRowConstants);
        gridRowConstants++;
    });
    divElement.appendChild(divElementTitle);
    divElement.appendChild(createResizer());
    if (gridRowInputs > 1 || gridRowOutputs > 1)
        divElement.appendChild(divParameters);
    if (gridRowConstants > 1)
        divElement.appendChild(divConstants);
    canvas.appendChild(divElement);
}

function createInputParameter(element, elementType, parameter, div, gridRow) {
    const divLabel = document.createElement('div');
    divLabel.setAttribute('class', 'parametertext textinput');
    divLabel.setAttribute('style', `grid-row: ${gridRow}`);
    divLabel.innerHTML = parameter.name;
    const divAnchor = document.createElement('div');
    divAnchor.setAttribute('class', `parameteranchor anchorinput ${parameter.type}`);
    divAnchor.setAttribute('alt', parameter.name);
    divAnchor.setAttribute('style', `grid-row: ${gridRow}`);
    divAnchor.dataset.id = `${elementType}.${element.id}.${parameter.code}`;
    divAnchor.dataset.type = `${parameter.type}`;
    divAnchor.innerHTML = '&nbsp;';
    div.appendChild(divAnchor);
    div.appendChild(divLabel);
}

function createConstantParameter(element, elementType, currentElement, parameter, div, gridRow) {
    const divLabel = document.createElement('div');
    divLabel.setAttribute('class', 'parametertext textconstant');
    divLabel.setAttribute('style', `grid-row: ${gridRow}`);
    divLabel.innerHTML = parameter.name;
    const input = document.createElement('input');
    input.setAttribute('class', 'inputconstant');
    input.setAttribute('type', parameter.type);
    input.setAttribute('style', `grid-row: ${gridRow}`);
    input.value = currentElement[parameter.code];
    input.dataset.id = `${elementType}.${element.id}.${parameter.code}`;
    input.dataset.type = `${parameter.type}`;
    div.appendChild(divLabel);
    div.appendChild(input);
}

function createOutputParameter(element, elementType, parameter, div, gridRow) {
    const divLabel = document.createElement('div');
    divLabel.setAttribute('class', 'parametertext textoutput');
    divLabel.setAttribute('style', `grid-row: ${gridRow}`);
    divLabel.innerHTML = parameter.name;
    const divAnchor = document.createElement('div');
    divAnchor.setAttribute('class', `parameteranchor anchoroutput ${parameter.type}`);
    divAnchor.setAttribute('alt', parameter.name);
    divAnchor.setAttribute('style', `grid-row: ${gridRow}`);
    divAnchor.dataset.id = `${elementType}.${element.id}.${parameter.code}`;
    divAnchor.dataset.type = `${parameter.type}`;
    divAnchor.innerHTML = '&nbsp;';
    div.appendChild(divLabel);
    div.appendChild(divAnchor);
}

function createRelations(relations) {
    connections.replaceChildren();
    relations.forEach((relation) => {
        createRelation(relation);
    });
}

function createRelation(relation) {
    const from = document.querySelector(`.anchoroutput[data-id='${relation.from}']`);
    const to = document.querySelector(`.anchorinput[data-id='${relation.to}']`);
    createConnection(from, to);
}

function createConnection(parameterFrom, parameterTo) {
    const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
    path.setAttribute('stroke', '#aaa');
    path.setAttribute('fill', 'none');
    path.setAttribute('stroke-width', '2');
    path.dataset.from = parameterFrom.dataset.id;
    path.dataset.to = parameterTo.dataset.id;
    connections.appendChild(path);
    updateConnections();
}

function updateConnections() {
    const paths = connections.querySelectorAll('path');
    canvasRect = canvas.getBoundingClientRect();
    paths.forEach((path) => {
        const from = document.querySelector(`.anchoroutput[data-id='${path.dataset.from}']`);
        const to = document.querySelector(`.anchorinput[data-id='${path.dataset.to}']`);
        if (from && to) {
            const rectFrom = from.getBoundingClientRect();
            const rectTo = to.getBoundingClientRect();

            const xFrom = rectFrom.left + rectFrom.width - canvasRect.left;
            const yFrom = rectFrom.top + (rectFrom.height / 2) - canvasRect.top;
            const xTo = rectTo.left - canvasRect.left;
            const yTo = rectTo.top + (rectTo.height / 2) - canvasRect.top;

            const dx = Math.abs(xTo - xFrom) / 2;
            const pathData = `M ${xFrom} ${yFrom} C ${xFrom + dx} ${yFrom}, ${xTo - dx} ${yTo}, ${xTo} ${yTo}`;
            path.setAttribute('d', pathData);
            path.setAttribute('class', `parameterpath ${from.dataset.type}`);
        }
    });
}

function addEventListener() {
    canvas.addEventListener('mousedown', (e) => {
        if (e.target.classList.contains('steptitle') || e.target.classList.contains('triggertitle')) {
            currentDragStep = e.target;
            currentDragStep.style.cursor = 'grabbing';
            currentDragStep.offsetX = parseInt(document.defaultView.getComputedStyle(currentDragStep.parentElement).left, 10) - e.clientX;
            currentDragStep.offsetY = parseInt(document.defaultView.getComputedStyle(currentDragStep.parentElement).top, 10) - e.clientY;
        }
        if (e.target.classList.contains('resizer')) {
            currentDragResizer = e.target;
            currentDragResizer.style.cursor = 'grabbing';
            currentDragResizer.offsetX = parseInt(document.defaultView.getComputedStyle(currentDragResizer.parentElement).width, 10) - e.clientX;
            currentDragResizer.offsetY = parseInt(document.defaultView.getComputedStyle(currentDragResizer.parentElement).height, 10) - e.clientY;
        }
        if (e.target.classList.contains('parameteranchor')) {
            if (e.target.classList.contains('anchoroutput')) {
                currentDragFrom = e.target
            } else {
                currentDragTo = e.target
                currentDragPath = document.querySelector(`.parameterpath[data-to='${currentDragTo.dataset.id}']`);
            }
            if (!currentDragPath) {
                currentDragPath = document.createElementNS('http://www.w3.org/2000/svg', 'path');
                currentDragPath.setAttribute('stroke', '#aaa');
                currentDragPath.setAttribute('fill', 'none');
                currentDragPath.setAttribute('stroke-width', '2');
                currentDragPath.setAttribute('class', `parameterpath type${e.target.dataset.type}`);
                if (currentDragFrom) {
                    currentDragPath.dataset.from = currentDragFrom.dataset.id;
                }
                if (currentDragTo) {
                    currentDragPath.dataset.to = currentDragTo.dataset.id;
                }
                connections.appendChild(currentDragPath);
            }
        }
    });

    canvas.addEventListener('mousemove', (e) => {
        if (currentDragStep) {
            const x = e.clientX + currentDragStep.offsetX;
            const y = e.clientY + currentDragStep.offsetY;
            currentDragStep.parentElement.style.left = `${x}px`;
            currentDragStep.parentElement.style.top = `${y}px`;
            updateConnections();
        }
        if (currentDragResizer) {
            const width = e.clientX + currentDragResizer.offsetX;
            const height = e.clientY + currentDragResizer.offsetY;
            currentDragResizer.parentElement.style.width = `${width}px`;
            currentDragResizer.parentElement.style.height = `${height}px`;
            updateConnections();
        }
        if (currentDragPath) {
            let xFrom = null;
            let yFrom = null;
            let xTo = null;
            let yTo = null;
            canvasRect = canvas.getBoundingClientRect();

            if (currentDragFrom) {
                const rectFrom = currentDragFrom.getBoundingClientRect();
                xFrom = rectFrom.left + rectFrom.width;
                yFrom = rectFrom.top + (rectFrom.height / 2);
                xTo = e.clientX;
                yTo = e.clientY;
            } else {
                const rectTo = currentDragTo.getBoundingClientRect();
                xFrom = e.clientX;
                yFrom = e.clientY;
                xTo = rectTo.left + rectTo.width;
                yTo = rectTo.top + (rectTo.height / 2);
            }

            xFrom -= canvasRect.left;
            yFrom -= canvasRect.top;
            xTo -= canvasRect.left;
            yTo -= canvasRect.top;

            const dx = Math.abs(xTo - xFrom) / 2;
            const pathData = `M ${xFrom} ${yFrom} C ${xFrom + dx} ${yFrom}, ${xTo - dx} ${yTo}, ${xTo} ${yTo}`;
            currentDragPath.setAttribute('d', pathData);
        }
    });

    canvas.addEventListener('mouseup', (e) => {
        if (currentDragStep) {
            currentDragStep.style.cursor = 'grab';
            currentDragStep = null;
        }
        if (currentDragResizer) {
            currentDragResizer.style.cursor = 'se-resize';
            currentDragResizer = null;
        }
        if (currentDragPath) {
            console.log(e.target);
            if (e.target.classList.contains('parameteranchor')) {
                if (e.target.classList.contains('anchoroutput') && currentDragTo) {
                    currentDragFrom = e.target
                } else if (e.target.classList.contains('anchorinput') && currentDragFrom) {
                    currentDragTo = e.target
                }

                if (currentDragFrom && currentDragTo) {
                    conf.relations.push({ from: currentDragFrom.dataset.id, to: currentDragTo.dataset.id });
                    createConnection(currentDragFrom, currentDragTo)
                }
            }
            connections.removeChild(currentDragPath);
            currentDragPath = null;
            currentDragFrom = null;
            currentDragTo = null;
        }
    });
}

function start() {
    canvas = document.getElementById('canvas');
    canvasRect = canvas.getBoundingClientRect();
    connections = document.getElementById('connections');
    connectionsRect = connections.getBoundingClientRect();
    tabs = document.querySelectorAll(".tab");
    getSteps();
    addEventListener();
}
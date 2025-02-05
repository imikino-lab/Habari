let canvas = null;
let canvasRect = null;
let connections = null;
let connectionsRect = null;
let currentDragStep = null;
let currentDragPath = null;
let currentDragFrom = null;
let currentDragTo = null;

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
            workflowsList['ask'].triggers.forEach((trigger) => {
                createTrigger(trigger);
                createSteps(trigger.steps);
                createRelations(trigger.relations);
            });
        });
}

function createTrigger(trigger) {
    let currentTrigger = triggersList.filter(item => { return item.code === trigger.code })[0];
    const divTrigger = document.createElement("div");
    divTrigger.setAttribute("class", "trigger");
    divTrigger.setAttribute("style", `top:${trigger.x}px; left:${trigger.y}px`);
    divTrigger.dataset.id = trigger.id;
    const divTriggerTitle = document.createElement("div");
    divTriggerTitle.setAttribute("class", "triggertitle");
    divTriggerTitle.innerHTML = currentTrigger.name;
    divTrigger.appendChild(divTriggerTitle);
    currentTrigger.inputs.forEach((input) => {
        createInputParameter(trigger, 'trigger', input, divTrigger);
    });
    currentTrigger.outputs.forEach((output) => {
        createOutputParameter(trigger, 'trigger', output, divTrigger);
    });
    canvas.appendChild(divTrigger);
}

function createSteps(steps) {
    steps.forEach((step) => {
        createStep(step);
    });
}

function createStep(step) {
    let currentStep = stepsList.filter(item => { return item.code === step.code })[0];
    const divStep = document.createElement("div");
    divStep.setAttribute("class", "step");
    divStep.setAttribute("style", `top:${step.x}px; left:${step.y}px`);
    divStep.dataset.id = step.id;
    const divStepTitle = document.createElement("div");
    divStepTitle.setAttribute("class", "steptitle");
    divStepTitle.innerHTML = currentStep.name;
    divStep.appendChild(divStepTitle);
    currentStep.inputs.forEach((input) => {
        createInputParameter(step, 'step', input, divStep);
    });
    currentStep.outputs.forEach((output) => {
        createOutputParameter(step, 'step', output, divStep);
    });
    canvas.appendChild(divStep);
}

function createInputParameter(element, elementType, parameter, div) {
    const divLabel = document.createElement("div");
    divLabel.setAttribute("class", 'parametertext textinput');
    divLabel.innerHTML = parameter.name;
    const divAnchor = document.createElement("div");
    divAnchor.setAttribute("class", `parameteranchor anchorinput ${parameter.type}`);
    divAnchor.setAttribute("alt", parameter.name);
    divAnchor.dataset.id = `${elementType}.${element.id}.${parameter.code}`;
    divAnchor.dataset.type = `${parameter.type}`;
    divAnchor.innerHTML = "&nbsp;";
    div.appendChild(divAnchor);
    div.appendChild(divLabel);
}

function createOutputParameter(element, elementType, parameter, div) {
    const divLabel = document.createElement("div");
    divLabel.setAttribute("class", 'parametertext textoutput');
    divLabel.innerHTML = parameter.name;
    const divAnchor = document.createElement("div");
    divAnchor.setAttribute("class", `parameteranchor anchoroutput ${parameter.type}`);
    divAnchor.setAttribute("alt", parameter.name);
    divAnchor.dataset.id = `${elementType}.${element.id}.${parameter.code}`;
    divAnchor.dataset.type = `${parameter.type}`;
    divAnchor.innerHTML = "&nbsp;";
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
    const from = document.querySelector(`.anchoroutput[data-id="${relation.from}"]`);
    const to = document.querySelector(`.anchorinput[data-id="${relation.to}"]`);
    createConnection(from, to);
}

function createConnection(parameterFrom, parameterTo) {
    const path = document.createElementNS("http://www.w3.org/2000/svg", "path");
    path.setAttribute("stroke", "#aaa");
    path.setAttribute("fill", "none");
    path.setAttribute("stroke-width", "2");
    path.dataset.from = parameterFrom.dataset.id;
    path.dataset.to = parameterTo.dataset.id;
    connections.appendChild(path);
    updateConnections();
}

function updateConnections() {
    const paths = connections.querySelectorAll("path");
    paths.forEach((path) => {
        const from = document.querySelector(`.anchoroutput[data-id="${path.dataset.from}"]`);
        const to = document.querySelector(`.anchorinput[data-id="${path.dataset.to}"]`);
        if (from && to) {
            const rectFrom = from.getBoundingClientRect();
            const rectTo = to.getBoundingClientRect();

            const xFrom = rectFrom.left + rectFrom.width - canvasRect.left;
            const yFrom = rectFrom.top + (rectFrom.height / 2) - canvasRect.top;
            const xTo = rectTo.left - canvasRect.left;
            const yTo = rectTo.top + (rectTo.height / 2) - canvasRect.top;

            const dx = Math.abs(xTo - xFrom) / 2;
            const pathData = `M ${xFrom} ${yFrom} C ${xFrom + dx} ${yFrom}, ${xTo - dx} ${yTo}, ${xTo} ${yTo}`;
            path.setAttribute("d", pathData);
            path.setAttribute("class", `parameterpath ${from.dataset.type}`);
        }
    });
}

function addEventListener() {
    canvas.addEventListener("mousedown", (e) => {
        if (e.target.classList.contains("steptitle") || e.target.classList.contains("triggertitle")) {
            currentDragStep = e.target.parentElement;
            currentDragStep.firstChild.style.cursor = "grabbing";
            currentDragStep.offsetX = e.offsetX + canvasRect.left;
            currentDragStep.offsetY = e.offsetY + canvasRect.top;
        }
        if (e.target.classList.contains("parameteranchor")) {
            if (e.target.classList.contains("anchoroutput")) {
                currentDragFrom = e.target
            } else {
                currentDragTo = e.target
                currentDragPath = document.querySelector(`.parameterpath[data-to="${currentDragTo.dataset.id}"]`);
            }

            if (!currentDragPath) {
                currentDragPath = document.createElementNS("http://www.w3.org/2000/svg", "path");
                currentDragPath.setAttribute("stroke", "#aaa");
                currentDragPath.setAttribute("fill", "none");
                currentDragPath.setAttribute("stroke-width", "2");
                currentDragPath.setAttribute("class", `parameterpath type${e.target.dataset.type}`);
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

    canvas.addEventListener("mousemove", (e) => {
        if (currentDragStep) {
            const x = e.clientX - currentDragStep.offsetX;
            const y = e.clientY - currentDragStep.offsetY;
            currentDragStep.style.left = `${x}px`;
            currentDragStep.style.top = `${y}px`;
            updateConnections();
        }

        if (currentDragPath) {
            let xFrom = null;
            let yFrom = null;
            let xTo = null;
            let yTo = null;

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
            currentDragPath.setAttribute("d", pathData);
        }
    });

    canvas.addEventListener("mouseup", (e) => {
        if (currentDragStep) {
            currentDragStep.firstChild.style.cursor = "grab";
            currentDragStep = null;
        }

        if (currentDragPath) {
            console.log(e.target);
            if (e.target.classList.contains("parameteranchor")) {
                if (e.target.classList.contains("anchoroutput") && currentDragTo) {
                    currentDragFrom = e.target
                } else if (e.target.classList.contains("anchorinput") && currentDragFrom) {
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
    canvas = document.getElementById("canvas");
    canvasRect = canvas.getBoundingClientRect();
    connections = document.getElementById("connections");
    connectionsRect = connections.getBoundingClientRect();
    getSteps();
    addEventListener();
}
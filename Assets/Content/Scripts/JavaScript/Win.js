// ### HOT RELOAD ###

const Clr = importNamespace("");
const Unity = importNamespace("UnityEngine");
const UI = importNamespace("UnityEngine.UIElements");
const UICommons = Clr.UICommons;

function Awake() {
/*    Unity.Debug.Log(Document.rootVisualElement.hierarchy[0].hierarchy[2]);
    var query = UICommons.Query(UI.Button, Document.rootVisualElement, "Finish");
    Unity.Debug.Log(query);
    UICommons.Query(UI.Button, Document.rootVisualElement, "Finish").clicked += () => {
        Unity.Debug.Log("Finish");
    };*/
    Document.rootVisualElement.hierarchy[0].hierarchy[2].clicked = func
}
function func() {
    Unity.Debug.Log("Finish");
}
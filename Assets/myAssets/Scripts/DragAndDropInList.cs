using UnityEngine;

public class DragAndDropInList : UIDragDropItem
{
    public GameObject prefab;
    protected override void Start()
    {
        base.Start();
    }    

    protected override void OnDragDropRelease(GameObject surface)
    {
        if (surface != null)
        {
            var dds = surface.GetComponent<ExampleDragDropSurface>();

            if (dds != null)
            {
                //var child = NGUITools.AddChild(dds.gameObject, prefab);
                //child.transform.localScale = dds.transform.localScale;

                //var trans = child.transform;
                //trans.position = UICamera.lastWorldPosition;
                //if (dds.rotatePlacedObject) trans.rotation = Quaternion.LookRotation(UICamera.lastHit.normal) * Quaternion.Euler(90f, 0f, 0f);

                base.OnDragDropRelease(surface);
                //NGUITools.Destroy(gameObject);
                DebugTWD.Log("Drop Realease " + dds.name);
                return;
            }
        }

        base.OnDragDropRelease(surface);
    }

    //protected override void OnDragStart()
    //{
    //    base.OnDragStart();

    //    this.GetComponent<UIWidget>().depth = 100;
    //}
    //protected override void OnDragEnd()
    //{
    //    base.OnDragEnd();

    //    this.GetComponent<UIWidget>().depth = 0;
    //}
}

using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public LayerMask Layer;
    IMousePointable last;
    private void Update()
    {
        if (last != null)
            last.WhileMouseIn();

        if (Physics.Raycast(new Ray(transform.position, transform.forward), out RaycastHit hit, 1000f, Layer))
        {
            if (hit.transform.TryGetComponent(out IMousePointable pointable))
            {
                if (last != null && last != pointable)
                    last.OnPointerExit();
                pointable.OnPointerEnter();
                last = pointable;
            }
        }
        else
        {
            if (last != null)
            {
                last.OnPointerExit();
                last = null;
            }
        }
    }
}
public interface IMousePointable
{
    void WhileMouseIn();
    void OnPointerEnter();
    void OnPointerExit();
}
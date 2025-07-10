using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingClickTarget : MonoBehaviour,  IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SelectBox.Instance != null)
        {
            Debug.Log("clicked");
            SelectBox.Instance.SendSelectedToBuilding(gameObject);
        }
    }
}

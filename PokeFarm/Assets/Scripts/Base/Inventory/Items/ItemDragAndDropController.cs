using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragAndDropController : MonoBehaviour
{
    [SerializeField] private ItemSlot draggingSlot;
    [SerializeField] private GameObject itemIcon;
    private RectTransform iconTransform;
    private Image iconImage;

    public void OnClick(ItemSlot otherSlot)
    {
        if (draggingSlot.item == null)
        {
            draggingSlot.Paste(otherSlot);
            otherSlot.Clear();
            UpdateIcon();
            return;
        }

        ItemSlot.Swap(draggingSlot, otherSlot);
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        itemIcon.SetActive(draggingSlot.item != null);

        if (draggingSlot.item != null)
            iconImage.sprite = draggingSlot.item.icon;
    }

    private void Start()
    {
        draggingSlot = new ItemSlot();
        iconTransform = itemIcon.GetComponent<RectTransform>();
        iconImage = itemIcon.GetComponent<Image>();
    }

    private void Update()
    {
        if (!itemIcon.activeInHierarchy) return;

        var worldPosition = (Vector2) Camera.main!.ScreenToWorldPoint(Input.mousePosition);
        iconTransform.position = worldPosition;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            ItemSpawnManager.Instance.SpawnItem(worldPosition, draggingSlot.item, draggingSlot.count);
            draggingSlot.Clear();
            UpdateIcon();
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class FlowchartScroll : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public ScrollRect scrollRect;

    // ドラッグ開始時のポインタ位置を保持する変数
    private Vector2 dragStartPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // ドラッグが始まったときに初期位置を記録
        dragStartPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ドラッグの移動量を取得
        float deltaX = eventData.position.x - dragStartPos.x;

        // ScrollRectの横方向のスクロールに移動量を適用
        scrollRect.horizontalNormalizedPosition -= deltaX / Screen.width;

        // ドラッグが続いても新しいドラッグ操作を可能にするために、現在のポインタ位置を次のドラッグ開始位置として更新
        dragStartPos = eventData.position;
    }
}
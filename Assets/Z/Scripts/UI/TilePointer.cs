using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Zio
{
    public class TilePointer :MonoBehaviour
    {
        [SerializeField] private Sprite arrowSprite;
        [SerializeField] private Sprite  crossSprite;
        [SerializeField] private GameObject pointerGameObject;
        [SerializeField] private  Image pointerImage;
        [SerializeField] private  RectTransform pointerRectTransform;

        public Vector3 TargetPosition { get; private set; }
        private Camera uiCamera;
        private Camera mainCamera;

        public void Init(Vector3 targetPosition,Camera cameraUi, Camera cameraMain)
        {
            TargetPosition = targetPosition;
            uiCamera = cameraUi;
            mainCamera = cameraMain;
            pointerImage.sprite = arrowSprite; 
        }

        public void Update()
        {
            var borderSize = 100f;
            var targetPositionScreenPoint = uiCamera.WorldToScreenPoint(TargetPosition);
            var isOffScreen = targetPositionScreenPoint.x <= borderSize ||
                              targetPositionScreenPoint.x >= Screen.width - borderSize ||
                              targetPositionScreenPoint.y <= borderSize ||
                              targetPositionScreenPoint.y >= Screen.height - borderSize;

            /*if (isOffScreen)
            {
                
            }*/
            
            if(isOffScreen)
            {
                pointerRectTransform.gameObject.SetActive(true);
                
                CanvasScreenSpace(isOffScreen, borderSize, targetPositionScreenPoint);
                //CanvasOverlayOffscreen(isOffScreen, borderSize, targetPositionScreenPoint);
            }
            else
            {
                
                pointerImage.sprite = crossSprite;
                var pointerWorldPosition = uiCamera.ScreenToWorldPoint(targetPositionScreenPoint);
                pointerRectTransform.position = pointerWorldPosition;
                var localPosition = pointerRectTransform.localPosition;
                localPosition = new Vector3(localPosition.x,
                    localPosition.y, 0f);
                pointerRectTransform.localPosition = localPosition;

                pointerRectTransform.localEulerAngles = Vector3.zero;
            }
        }

        private void CanvasScreenSpace(bool offScreen, float borderSize, Vector3 targetPositionScreenPoint)
        {
            RotatePointerTowardsTargetPosition();

            pointerImage.sprite = arrowSprite;
            var cappedTargetScreenPosition = targetPositionScreenPoint;
            cappedTargetScreenPosition.x =
                Mathf.Clamp(cappedTargetScreenPosition.x, borderSize, Screen.width - borderSize);
            cappedTargetScreenPosition.y =
                Mathf.Clamp(cappedTargetScreenPosition.y, borderSize, Screen.height - borderSize);

            var pointerWorldPosition = uiCamera.ScreenToWorldPoint(cappedTargetScreenPosition);
            pointerRectTransform.position = pointerWorldPosition;
            var localPosition = pointerRectTransform.localPosition;
            localPosition = new Vector3(localPosition.x,
                localPosition.y, 0f);
            pointerRectTransform.localPosition = localPosition;
        }
        private void CanvasOverlayOffscreen(bool offScreen, float borderSize, Vector3 targetPositionScreenPoint)
        {
            RotatePointerTowardsTargetPosition();
            var cappedTargetScreenPosition = targetPositionScreenPoint;
            cappedTargetScreenPosition.x =
                Mathf.Clamp(cappedTargetScreenPosition.x, borderSize, Screen.width - borderSize);
            cappedTargetScreenPosition.y =
                Mathf.Clamp(cappedTargetScreenPosition.y, borderSize, Screen.height - borderSize);

            pointerRectTransform.position = cappedTargetScreenPosition;
            var localPosition = pointerRectTransform.localPosition;
            localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
            pointerRectTransform.localPosition = localPosition;
        }

        private void RotatePointerTowardsTargetPosition()
        {
            var fromPosition = mainCamera.transform.position;
            fromPosition.z = 0f;
            var dir = (TargetPosition - fromPosition).normalized;
            var angle = UtilsClass.GetAngleFromVectorFloat(dir);
            pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
        }

        public void DestroySelf()
        {
            Destroy(pointerGameObject);
        }
    
    }
}
using UnityEngine;

namespace Interactive
{
    public class GrabsController : MonoBehaviour
    {
        private static Transform _lGrab;
        private static Transform _rGrab;

        private void Start()
        {
            _lGrab = transform.GetChild(0).GetComponent<Transform>();
            _rGrab = transform.GetChild(1).GetComponent<Transform>();
        }

        #region Grab

        public static void GrabLeft(Transform childTransform)
        {
            LetGoLeftGrab();
            Grab(childTransform, _lGrab);
        }

        public static void GrabRight(Transform childTransform)
        {
            LetGoRightGrab();
            Grab(childTransform, _rGrab);
        }

        private static void Grab(Transform childTransform, Transform grabTransform)
        {
            childTransform.parent = grabTransform;
            childTransform.localPosition = Vector3.zero;
            childTransform.localRotation = Quaternion.identity;
            childTransform.localScale = Vector3.one;
        }

        #endregion

        #region LetGo

        public static void LetGo()
        {
            LetGoLeftGrab();
            LetGoRightGrab();
        }

        public static void LetGoLeftGrab() => LetGo(_lGrab);

        public static void LetGoRightGrab() => LetGo(_rGrab);

        private static void LetGo(Transform grabTransform)
        {
            if (grabTransform.childCount > 0)
                grabTransform.GetChild(0).transform.parent = null;
        }

        #endregion

        #region Remove

        public static void RemoveChildrenLeft() => RemoveChildren(_lGrab);
        public static void RemoveChildrenRight() => RemoveChildren(_rGrab);

        public static void RemoveChildrens()
        {
            RemoveChildrenLeft();
            RemoveChildrenRight();
        }
        private static void RemoveChildren(Transform grabTransform)
        {
            if (grabTransform.childCount > 0)
                Destroy(grabTransform.GetChild(0).gameObject);
        }

        #endregion
    }
}
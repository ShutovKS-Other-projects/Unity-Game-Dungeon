using Old.Magic.Type;
using Unity.VisualScripting;
using UnityEngine;

namespace Old.Magic.SuperMagic
{
    public abstract class MagicAttack : Magic
    {
        #region Fields

        public override MagicType MagicType { get; set; }
        protected override Color ColorSphere { get; set; }
        protected abstract float ForceFlight { get; set; }
        protected abstract float Radius { get; set; }

        #endregion

        public void Enter()
        {
            CreateMagicModel();
        }

        #region Unity Methods

        private void Start()
        {
            CreateMagicModel();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            StartCoroutine(MagicDestroy());
        }

        #endregion

        #region Methods

        protected override void CreateMagicModel()
        {
            var cameraTransform = UnityEngine.Camera.main!.transform;
            var magicPosition = cameraTransform!.position;
            magicPosition += cameraTransform!.forward * 1;

            transform.position = magicPosition;
            transform.localScale = new Vector3(Radius, Radius, Radius);
            transform.rotation = cameraTransform.rotation;

            AddCollider();
            AddRigidbody();
            SetColor();

            StartCoroutine(MagicDestroy(3f));
        }

        private void AddRigidbody()
        {
            var rb = transform.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.AddForce(transform.forward * ForceFlight);
        }

        private void AddCollider()
        {
            var sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = Radius;
        }

        #endregion
    }
}
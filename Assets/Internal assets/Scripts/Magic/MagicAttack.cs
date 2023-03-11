using Magic.Object;
using Magic.Type;
using Skill.Enum;
using Unity.VisualScripting;
using UnityEngine;

namespace Magic
{
    public sealed class MagicAttack : Magic
    {
        #region Fields

        private MagicAttackObject _magicAttackObject;
        private float ForceFlight { get; set; }
        private float Radius { get; set; }

        protected override MagicAttackType MagicAttackType { get; set; }
        protected override Color ColorSphere { get; set; }

        #endregion

        #region Unity Methods

        protected override void OnTriggerEnter(Collider other)
        {
            StartCoroutine(MagicDestroy());
        }

        #endregion

        #region Methods

        public void SetParameters(MagicAttackObject magicAttackObject)
        {
            _magicAttackObject = magicAttackObject;
            ForceFlight = _magicAttackObject.ForceFlight;
            Radius = _magicAttackObject.Radius;
            MagicAttackType = _magicAttackObject.MagicAttackType;
            ColorSphere = _magicAttackObject.ColorSphere;

            CreateMagicModel();
        }

        protected override void CreateMagicModel()
        {
            var cameraTransform = UnityEngine.Camera.main!.transform;
            var magicTransform = transform;
            var magicPosition = cameraTransform!.position;
            magicPosition += cameraTransform!.forward * 1;

            magicTransform.position = magicPosition;
            magicTransform.localScale = new Vector3(Radius, Radius, Radius);
            magicTransform.rotation = cameraTransform.rotation;

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
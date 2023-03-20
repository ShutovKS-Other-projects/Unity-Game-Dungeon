using System.Collections;
using Old.Magic.Type;
using UnityEngine;

namespace Old.Magic.SuperMagic
{
    public abstract class Magic : MonoBehaviour
    {
        public abstract MagicType MagicType { get; set; }
        protected abstract Color ColorSphere { get; set; }

        protected abstract void OnTriggerEnter(Collider other);

        protected abstract void CreateMagicModel();

        protected void SetColor() => GetComponent<Renderer>().material.color = ColorSphere;

        protected IEnumerator MagicDestroy()
        {
            yield return null;
            Destroy(gameObject);
        }

        /// <summary> Уничтожение магии </summary>
        /// <param name="time"> Время до уничтожения </param>
        protected IEnumerator MagicDestroy(float time = 0)
        {
            switch (time)
            {
                case < 0:
                    Destroy(gameObject);
                    break;

                case 0:
                    yield return null;
                    Destroy(gameObject);
                    break;

                case > 0:
                    yield return new WaitForSeconds(time);
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
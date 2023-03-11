using System;
using System.Collections;
using Magic.Type;
using Skill.Enum;
using UnityEngine;

namespace Magic
{
    public abstract class Magic : MonoBehaviour
    {
        protected abstract MagicAttackType MagicAttackType { get; set; }
        protected abstract Color ColorSphere { get; set; }

        protected abstract void OnTriggerEnter(Collider other);

        protected abstract void CreateMagicModel();

        protected void SetColor() => GetComponent<Renderer>().material.color = ColorSphere;
        
        protected IEnumerator MagicDestroy()
        {
            yield return null;
            Destroy(gameObject);
        }

        protected IEnumerator MagicDestroy(float time = 0)
        {
            if (time <= 0) throw new ArgumentOutOfRangeException(nameof(time));
            yield return new WaitForSeconds(time);
            Destroy(gameObject);
        }
        
    }
}
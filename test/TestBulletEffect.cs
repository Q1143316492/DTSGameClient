using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.test
{
    public class TestBulletEffect : MonoBehaviour
    {
        public float speed = 1f;

        void Start()
        {

        }

        void Update()
        {
            Vector3 vector = gameObject.transform.position;
            vector.y += speed;
            gameObject.transform.position = vector;
        }
    }
}

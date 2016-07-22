using UnityEngine;
using System.Collections.Generic;

    /// <summary>
    /// 描述：
    /// author： 
    /// </summary>
	[AddComponentMenu("DajiaGame/Px/ TestPosScript ")]
    public class TestPosScript : MonoBehaviour
    {
        public static TestPosScript instance;

        public Vector3 DirVector3;
        public float left;
        public float right;

        void Awake()
        {
            instance = this;
        }
    }

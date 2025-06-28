using UnityEngine;

namespace Scripts
{
    public interface Test
    {
        void GetAction();
    }

    public class Test1 : Test
    {
        public void GetAction()
        {
            Debug.Log("Test1");
        }
    }
}
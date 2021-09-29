using UnityEngine;

public class BodyController : MonoBehaviour
{
    bool _kill = false;
    void Update()
    {           
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (!_kill)
        {
            if (Physics.Raycast(ray, out hit))
            {
                var newPos = hit.point;
                transform.position = new Vector3(newPos.x, 2, newPos.z);
            }
        }
    }
    public void GetDamage(bool count)
    {
        _kill = count;       
    }       
    
}
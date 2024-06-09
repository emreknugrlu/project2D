using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordClash : MonoBehaviour
{
    [Header("Clash Settings")] 
    public float playerPower = 2.0f;
    public float clashDifficulty = 6.1f;
    public float sliderPower = 1.0f;
    public float currentDistance = 1.0f;
    
    [Header("Components")]
    [SerializeField] private RectTransform sword;
    [SerializeField] private RectTransform greenArea;
    [SerializeField] public Slider slider;
    
    
    
    private Vector2 midPosition;
    private float greenLength;
    
    // Start is called before the first frame update
    void Start()
    {
        greenLength = greenArea.sizeDelta.x;
        midPosition = greenArea.position;
    }

    // Update is called once per frame
    void Update()
    {
        currentDistance = sword.position.x - midPosition.x;
        if (currentDistance < -150f || currentDistance > 150f)
        {
            Debug.Log("Oyunu Kaybettin");
            this.enabled = false;
            return;
        }

        if (slider.value > 0.999f)
        {
            Debug.Log("Oyunu Kazand�n");
            this.enabled = false;
            return;
        }
            
        
        
        float bob1 = 0;
        if (Input.GetKeyDown(KeyCode.A)) bob1 = -1;
        if (Input.GetKeyDown(KeyCode.D)) bob1 = 1;
        sword.Translate(bob1 * Time.deltaTime * 1000* playerPower, 0f, 0f,greenArea);

        

        float toMoveAbs =Mathf.Sqrt(Mathf.Abs(currentDistance)) * Time.deltaTime * clashDifficulty ;
        float direciton = currentDistance / Mathf.Abs(currentDistance);
        Debug.Log(toMoveAbs);
        //toMoveAbs = Mathf.Clamp(toMoveAbs, 0.05f, 5);
        float toMove = toMoveAbs * direciton;

        if (Mathf.Abs(currentDistance) < greenLength / 2)
            slider.value += sliderPower * Time.deltaTime;
        else
            slider.value -= 2 * sliderPower * Time.deltaTime;


        
        sword.Translate(toMove, 0f, 0f,greenArea);
        
        
        currentDistance =Mathf.Clamp(currentDistance, -150, 150);
        sword.position.Set(currentDistance, 0f, 0f);




    }
}

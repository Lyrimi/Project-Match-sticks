using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FireSystem : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public int Duration;
    public Slider matchSlider;
    int curLength = 0;
    public int curSticks = 0;
    public Animator anim;
    public bool Sprint;
    public int SprintDecay;
    BoxCollider2D box;


    // Start is called before the first frame update
    void Start()
    {
        matchSlider.maxValue = Duration;
        matchSlider.value = Duration;
        curLength = Duration;
        Text.text = curSticks.ToString();
        box = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (curLength > 0)
        {
            if (Sprint)
            {
                curLength -= SprintDecay;
            }
            else
            {
                curLength--;
            }
            matchSlider.value = curLength;
            anim.SetBool("Alive", true);
        }
        else if (curSticks > 0)
        {
            curSticks--;
            curLength = Duration;
            Text.text = curSticks.ToString();
        }
        else 
        {
            print("you fucking died haha");
            anim.SetBool("Alive", false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stick"))
        {
            curSticks++;
            Destroy(collision.gameObject);
            Text.text = curSticks.ToString();
        }
    }
}

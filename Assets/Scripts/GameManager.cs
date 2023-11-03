using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public enum Phase { FISHING, PREREEL, REELING, RECIPE }

public class GameManager : MonoBehaviour
{
    private Camera mainCamera;

    public Phase state;

    public float Money = 000f;
    public Text moneyText;
    public Slider time;

    public GameObject recBook;
    public Sprite RBP1;
    public Sprite RBP2;
    public Sprite RBP3;
    public int page = 1;

    public GameObject Rod;
    public GameObject Hook;
    public Collider2D hookCol;
    public LineRenderer lineRend;

    public GameObject hkFish;
    public float reel = 0;

    public GameObject reelGame;
    public GameObject reelGameTop;
    public GameObject reelGameBot;
    public GameObject reelGameSlide;
    public bool freshReel;
    public float reelPass;

    public GameObject Q1;
    public GameObject Q2;
    public GameObject Q3;
    public int Qpos = 0;

    public GameObject inQ1;
    public GameObject inQ2;
    public GameObject inQ3;

    public GameObject L1L;
    public GameObject L1R;
    public bool L1Side = false;

    public GameObject L2L;
    public GameObject L2R;
    public bool L2Side = false;

    public GameObject L3L;
    public GameObject L3R;
    public bool L3Side = false;

    public GameObject L4L;
    public GameObject L4R;
    public bool L4Side = false;

    public GameObject L5L;
    public GameObject L5R;
    public bool L5Side = false;

    public GameObject Tuna;
    public GameObject Salmon;
    public GameObject Yellowtail;
    public GameObject Eel;
    public GameObject Crab;

    public GameObject SpicyTuna;
    public GameObject Alaska;
    public GameObject Dynamite;
    public GameObject California;
    public GameObject Caterpillar;

    public GameObject IV1;
    public GameObject IV2;
    public GameObject IV3;
    public GameObject IV4;
    public GameObject IV5;
    public GameObject IV6;
    public GameObject IV7;
    public GameObject IV8;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        lineRend = GetComponent<LineRenderer>();
        hookCol = Hook.GetComponent<Collider2D>();

        time.maxValue = 120;
        time.value = 120;

        state = Phase.FISHING;
        StartCoroutine(QueueFill(5));
        StartCoroutine(Countdown());

        StartCoroutine(Lane1());
        StartCoroutine(Lane2());
        StartCoroutine(Lane3());
        StartCoroutine(Lane4());
        StartCoroutine(Lane5());

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (state == Phase.FISHING) {

            if (mousePos.y < Rod.transform.position.y && mousePos.y > L5L.transform.position.y) {
                Hook.transform.position = new Vector3(-5f, mousePos.y);
            }

        }

        if (state == Phase.PREREEL)
        {
            if (Input.GetKeyDown(KeyCode.Space)) {

                if (freshReel)
                {
                    if (reelPass < 0.2f || reelPass > 0.8f) {
                        Debug.Log("Miss");

                        Destroy(hkFish);
                        hkFish = null;
                        state = Phase.FISHING;
                    }
                    if (reelPass >= 0.2f && reelPass < 0.4f || reelPass <= 0.8f && reelPass > 0.6f)
                    {
                        Debug.Log("Almost");
                        state = Phase.REELING;
                        StartCoroutine(MoveToPosition(Hook.transform.position, Rod.transform.position, 2));
                    }
                    if (reelPass >= 0.4f && reelPass <= 0.6f) {
                        Debug.Log("Perfect");
                        state = Phase.REELING;
                        StartCoroutine(MoveToPosition(Hook.transform.position, Rod.transform.position, 0.5f));
                    }
                    freshReel = false;
                    reelGame.SetActive(false);


                }


            }
        }

        if (state == Phase.RECIPE) {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                if (page == 2) {
                    page = 1;
                    recBook.GetComponent<Image>().sprite = RBP1;
                } else if (page == 3)
                {
                    page = 2;
                    recBook.GetComponent<Image>().sprite = RBP2;
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (page == 2)
                {
                    page = 3;
                    recBook.GetComponent<Image>().sprite = RBP3;
                }
                else if (page == 1)
                {
                    page = 2;
                    recBook.GetComponent<Image>().sprite = RBP2;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                state = Phase.FISHING;
                recBook.SetActive(false);
            }

        }

        lineRend.SetPosition(0, Rod.transform.position);
        lineRend.SetPosition(1, Hook.transform.position);

        moneyText.text = ("" + Money);

    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;
        if (rayHit.collider.gameObject.tag != "Prepare") return;

        if (rayHit.collider.gameObject.GetComponentInParent<RecipeReq>().ready == true)
        {
            Debug.Log("PREPARED");
            OrderComplete(rayHit.collider.gameObject.transform.parent.gameObject);
        }
    }
    //Fishing / Reeling
    public void FishCaught(GameObject Fish) {
        hkFish = Fish;
        Fish.GetComponent<FishSwim>().hook = Hook;
        Fish.GetComponent<FishSwim>().Hooked();
        reel = Fish.GetComponent<FishSwim>().depth;
        state = Phase.PREREEL;
        StartCoroutine(Reeling());
    }

    public void FishBagged() {
        FishDepo();
        StartCoroutine(Cooldown());
        Destroy(hkFish);
        hkFish = null;
        reel = 0;
        reelPass = 0;
        freshReel = false;
    }

    IEnumerator Cooldown() {
        yield return new WaitForSeconds(0.5f);
        state = Phase.FISHING;

    }

    IEnumerator Reeling() {
        freshReel = true;
        bool up = true;
        //bool reelLoop = true;
        reelGame.SetActive(true);
        reelGameSlide.transform.position = reelGameBot.transform.position;
        float curTime = 0;
        while (freshReel) {
            while (curTime < 1)
            {
                if (up)
                {
                    reelGameSlide.transform.position = Vector3.Lerp(reelGameBot.transform.position, reelGameTop.transform.position, curTime / 1);
                    Debug.Log("going up!");
                }
                if (!up)
                {
                    reelGameSlide.transform.position = Vector3.Lerp(reelGameTop.transform.position, reelGameBot.transform.position, curTime / 1);
                    Debug.Log("going down!");
                }
                curTime += Time.deltaTime;
                reelPass += Time.deltaTime;
                yield return null;
            }
            if (curTime >= 1)
            {
                curTime = 0;
                reelPass = 0;
                if (up)
                {
                    up = false;
                } else if (!up)
                {
                    up = true;
                }
            }
        }

    }

    IEnumerator MoveToPosition(Vector3 startPos, Vector3 endPos, float moveTime)
    {
        float curTime = 0;
        while (curTime < moveTime)
        {
            Hook.transform.position = Vector3.Lerp(startPos, endPos, curTime / moveTime);
            curTime += Time.deltaTime;
            yield return null;
        }
        if (curTime >= moveTime) {
            FishBagged();
        }
    }

    // Order / Queue

    IEnumerator QueueFill(float wait) {
        yield return new WaitForSeconds(wait);
        if (Qpos < 2)
        {
            NewOrder();
            StartCoroutine(QueueFill(5));
        }
        else if (Qpos == 2)
        {
            NewOrder();
        }

    }

    public void NewOrder() {
        Qpos += 1;

        int ordType = Random.Range(0, 5);
        GameObject toSpawn = SpicyTuna;
        Vector3 spawnPos = transform.position;

        switch (ordType)
        {
            case 0:
                toSpawn = SpicyTuna;
                break;

            case 1:
                toSpawn = Alaska;
                break;

            case 2:
                toSpawn = California;
                break;

            case 3:
                toSpawn = Caterpillar;
                break;

            case 4:
                toSpawn = Dynamite;
                break;
        }

        switch (Qpos)
        {
            case 1:
                spawnPos = Q1.transform.position;
                inQ1 = Instantiate(toSpawn, spawnPos, Quaternion.identity);
                StartCoroutine(OrderTickDown(inQ1));
                break;

            case 2:
                spawnPos = Q2.transform.position;
                inQ2 = Instantiate(toSpawn, spawnPos, Quaternion.identity);
                StartCoroutine(OrderTickDown(inQ2));
                break;

            case 3:
                spawnPos = Q3.transform.position;
                inQ3 = Instantiate(toSpawn, spawnPos, Quaternion.identity);
                StartCoroutine(OrderTickDown(inQ3));
                break;
        }
        OrdCheck();
    }

    public void FishDepo() {

        if (IV1.GetComponent<InventorySlot>().empty == true)
        {
            IV1.GetComponent<InventorySlot>().empty = false;
            IV1.GetComponent<InventorySlot>().iv_fishtype = hkFish.GetComponent<FishSwim>().fishType;
        }
        else
        {
            if (IV2.GetComponent<InventorySlot>().empty == true)
            {
                IV2.GetComponent<InventorySlot>().empty = false;
                IV2.GetComponent<InventorySlot>().iv_fishtype = hkFish.GetComponent<FishSwim>().fishType;
            }
            else
            {
                if (IV3.GetComponent<InventorySlot>().empty == true)
                {
                    IV3.GetComponent<InventorySlot>().empty = false;
                    IV3.GetComponent<InventorySlot>().iv_fishtype = hkFish.GetComponent<FishSwim>().fishType;
                }
                else
                {
                    if (IV4.GetComponent<InventorySlot>().empty == true)
                    {
                        IV4.GetComponent<InventorySlot>().empty = false;
                        IV4.GetComponent<InventorySlot>().iv_fishtype = hkFish.GetComponent<FishSwim>().fishType;
                    }
                    else
                    {
                        if (IV5.GetComponent<InventorySlot>().empty == true)
                        {
                            IV5.GetComponent<InventorySlot>().empty = false;
                            IV5.GetComponent<InventorySlot>().iv_fishtype = hkFish.GetComponent<FishSwim>().fishType;
                        }
                        else
                        {
                            if (IV6.GetComponent<InventorySlot>().empty == true)
                            {
                                IV6.GetComponent<InventorySlot>().empty = false;
                                IV6.GetComponent<InventorySlot>().iv_fishtype = hkFish.GetComponent<FishSwim>().fishType;
                            }
                            else
                            {
                                if (IV7.GetComponent<InventorySlot>().empty == true)
                                {
                                    IV7.GetComponent<InventorySlot>().empty = false;
                                    IV7.GetComponent<InventorySlot>().iv_fishtype = hkFish.GetComponent<FishSwim>().fishType;
                                }
                                else
                                {
                                    if (IV8.GetComponent<InventorySlot>().empty == true)
                                    {
                                        IV8.GetComponent<InventorySlot>().empty = false;
                                        IV8.GetComponent<InventorySlot>().iv_fishtype = hkFish.GetComponent<FishSwim>().fishType;
                                    }

                                }

                            }

                        }

                    }

                }

            }

        }
        OrdCheck();

    }

    public void OrdCheck()
    {
        int QIV = 0;
        int i;
        GameObject toCheck = null;
        for (i = 0; i < 3; i++)
        {
            QIV = 0;
            switch (i) {
                case 0:
                    if (inQ1 != null)
                    {
                        toCheck = inQ1;
                    }
                    break;
                case 1:
                    if (inQ2 != null)
                    {
                        toCheck = inQ2;
                    }
                    break;
                case 2:
                    if (inQ3 != null)
                    {
                        toCheck = inQ3;
                    }
                    break;
            }
            if (toCheck != null)
            {
                toCheck.GetComponent<RecipeReq>().hasFish1 = false;
                toCheck.GetComponent<RecipeReq>().hasFish2 = false;
                if (toCheck.GetComponent<RecipeReq>().fishType1 == IV1.GetComponent<InventorySlot>().iv_fishtype)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish1 = true;
                    toCheck.GetComponent<RecipeReq>().Fish1IV = IV1;
                    QIV = 1;
                }
                if (toCheck.GetComponent<RecipeReq>().fishType1 == IV2.GetComponent<InventorySlot>().iv_fishtype)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish1 = true;
                    toCheck.GetComponent<RecipeReq>().Fish1IV = IV2;
                    QIV = 2;
                }
                if (toCheck.GetComponent<RecipeReq>().fishType1 == IV3.GetComponent<InventorySlot>().iv_fishtype)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish1 = true;
                    toCheck.GetComponent<RecipeReq>().Fish1IV = IV3;
                    QIV = 3;
                }
                if (toCheck.GetComponent<RecipeReq>().fishType1 == IV4.GetComponent<InventorySlot>().iv_fishtype)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish1 = true;
                    toCheck.GetComponent<RecipeReq>().Fish1IV = IV4;
                    QIV = 4;
                }
                if (toCheck.GetComponent<RecipeReq>().fishType1 == IV5.GetComponent<InventorySlot>().iv_fishtype)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish1 = true;
                    toCheck.GetComponent<RecipeReq>().Fish1IV = IV5;
                    QIV = 5;
                }
                if (toCheck.GetComponent<RecipeReq>().fishType1 == IV6.GetComponent<InventorySlot>().iv_fishtype)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish1 = true;
                    toCheck.GetComponent<RecipeReq>().Fish1IV = IV6;
                    QIV = 6;
                }
                if (toCheck.GetComponent<RecipeReq>().fishType1 == IV7.GetComponent<InventorySlot>().iv_fishtype)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish1 = true;
                    toCheck.GetComponent<RecipeReq>().Fish1IV = IV7;
                    QIV = 7;
                }
                if (toCheck.GetComponent<RecipeReq>().fishType1 == IV8.GetComponent<InventorySlot>().iv_fishtype)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish1 = true;
                    toCheck.GetComponent<RecipeReq>().Fish1IV = IV8;
                    QIV = 8;
                }

                //Checking 2nd Fish Requirement

                if (toCheck.GetComponent<RecipeReq>().fishType2 == IV1.GetComponent<InventorySlot>().iv_fishtype && QIV != 1)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish2 = true;
                    toCheck.GetComponent<RecipeReq>().Fish2IV = IV1;

                }
                if (toCheck.GetComponent<RecipeReq>().fishType2 == IV2.GetComponent<InventorySlot>().iv_fishtype && QIV != 2)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish2 = true;
                    toCheck.GetComponent<RecipeReq>().Fish2IV = IV2;

                }
                if (toCheck.GetComponent<RecipeReq>().fishType2 == IV3.GetComponent<InventorySlot>().iv_fishtype && QIV != 3)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish2 = true;
                    toCheck.GetComponent<RecipeReq>().Fish2IV = IV3;

                }
                if (toCheck.GetComponent<RecipeReq>().fishType2 == IV4.GetComponent<InventorySlot>().iv_fishtype && QIV != 4)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish2 = true;
                    toCheck.GetComponent<RecipeReq>().Fish2IV = IV4;

                }
                if (toCheck.GetComponent<RecipeReq>().fishType2 == IV5.GetComponent<InventorySlot>().iv_fishtype && QIV != 5)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish2 = true;
                    toCheck.GetComponent<RecipeReq>().Fish2IV = IV5;

                }
                if (toCheck.GetComponent<RecipeReq>().fishType2 == IV6.GetComponent<InventorySlot>().iv_fishtype && QIV != 6)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish2 = true;
                    toCheck.GetComponent<RecipeReq>().Fish2IV = IV6;

                }
                if (toCheck.GetComponent<RecipeReq>().fishType2 == IV7.GetComponent<InventorySlot>().iv_fishtype && QIV != 7)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish2 = true;
                    toCheck.GetComponent<RecipeReq>().Fish2IV = IV7;

                }
                if (toCheck.GetComponent<RecipeReq>().fishType2 == IV8.GetComponent<InventorySlot>().iv_fishtype && QIV != 8)
                {
                    toCheck.GetComponent<RecipeReq>().hasFish2 = true;
                    toCheck.GetComponent<RecipeReq>().Fish2IV = IV8;

                }

            }
        }

    }

    IEnumerator OrderTickDown(GameObject Order) 
    {
        if (Order != null) {
            while (Order.GetComponent<RecipeReq>().timeLeft < 20)
            {
                if (Order != null)
                {
                    Order.GetComponent<RecipeReq>().timeLeft += Time.deltaTime;
                    yield return null;
                }
            }
            if (Order.GetComponent<RecipeReq>().timeLeft >= 20)
            {
                OrderExpire(Order);
            }
        }
    }
    public void OrderExpire(GameObject Order)
    {
        Qpos -= 1;
        if (Order == inQ1)
        {
            Destroy(inQ1);
            if (inQ2 != null)
            {
                inQ2.transform.position = Q1.transform.position;
                inQ1 = inQ2;
                inQ2 = null;
            }

            if (inQ3 != null)
            {
                inQ3.transform.position = Q2.transform.position;
                inQ2 = inQ3;
                inQ3 = null;
                StartCoroutine(QueueFill(2));
            }
        }
        if (Order == inQ2)
        {
            Destroy(inQ2);
            if (inQ3 != null)
            {
                inQ3.transform.position = Q2.transform.position;
                inQ2 = inQ3;
                inQ3 = null;
                StartCoroutine(QueueFill(2));
            }
        }
        if (Order == inQ3)
        {
            Destroy(inQ3);
            StartCoroutine(QueueFill(2));
        }
        OrdCheck();
    }
    public void OrderComplete(GameObject Order) {
        Money += (10 + (25 - Mathf.RoundToInt(Order.GetComponent<RecipeReq>().timeLeft)));
        Qpos -= 1;
        Order.GetComponent<RecipeReq>().Fish1IV.GetComponent<InventorySlot>().iv_fishtype = 0;
        Order.GetComponent<RecipeReq>().Fish1IV.GetComponent<InventorySlot>().empty = true;
        Order.GetComponent<RecipeReq>().Fish2IV.GetComponent<InventorySlot>().iv_fishtype = 0;
        Order.GetComponent<RecipeReq>().Fish2IV.GetComponent<InventorySlot>().empty = true;
        if (Order == inQ1) {
            Destroy(inQ1);
            if (inQ2 != null) {
                inQ2.transform.position = Q1.transform.position;
                inQ1 = inQ2;
                inQ2 = null;
            }

            if (inQ3 != null)
            {
                inQ3.transform.position = Q2.transform.position;
                inQ2 = inQ3;
                inQ3 = null;
                StartCoroutine(QueueFill(2));
            }
        }
        if (Order == inQ2)
        {
            Destroy(inQ2);
            if (inQ3 != null)
            {
                inQ3.transform.position = Q2.transform.position;
                inQ2 = inQ3;
                inQ3 = null;
                StartCoroutine(QueueFill(2));
            }
        }
        if (Order == inQ3) 
        {
            Destroy(inQ3);
            StartCoroutine(QueueFill(2));
        }
        OrdCheck();
    }

    public void RecipeOpen() {
        if (state == Phase.FISHING) {
            state = Phase.RECIPE;
            recBook.GetComponent<Image>().sprite = RBP1;
            recBook.SetActive(true);
            page = 1;
        }
    
    }

    IEnumerator Lane1() {
        GameObject fish;
        GameObject spawn;
        GameObject oppspawn;
        GameObject toSpawn = Tuna;

        int fishtype = Random.Range(1, 5);
        switch (fishtype)
        {
            case 1:
                toSpawn = Tuna;
                break;

            case 2:
                toSpawn = Salmon;
                break;

            case 3:
                toSpawn = Yellowtail;
                break;

            case 4:
                toSpawn = Eel;
                break;
        }

        if (L1Side)
        {
            spawn = L1L;
            oppspawn = L1R;
            L1Side = false;
        } else 
        {
            spawn = L1R;
            oppspawn = L1L;
            L1Side = true;
        }
        fish = Instantiate(toSpawn, new Vector3(spawn.transform.position.x, spawn.transform.position.y), Quaternion.identity);
        fish.GetComponent<FishSwim>().OppSpawn = oppspawn;
        fish.GetComponent<FishSwim>().left = L1Side;
        fish.GetComponent<FishSwim>().depth = 1;
        fish.GetComponent<FishSwim>().speed = 2;
        fish.GetComponent<FishSwim>().fishType = fishtype;

        yield return new WaitForSeconds(5);

        StartCoroutine(Lane1());
    }

    IEnumerator Lane2()
    {
        GameObject fish;
        GameObject spawn;
        GameObject oppspawn;
        GameObject toSpawn = Tuna;

        int fishtype = Random.Range(1, 5);
        switch (fishtype)
        {
            case 1:
                toSpawn = Tuna;
                break;

            case 2:
                toSpawn = Salmon;
                break;

            case 3:
                toSpawn = Yellowtail;
                break;

            case 4:
                toSpawn = Eel;
                break;
        }

        if (L2Side)
        {
            spawn = L2L;
            oppspawn = L2R;
            L2Side = false;
        }
        else
        {
            spawn = L2R;
            oppspawn = L2L;
            L2Side = true;
        }
        fish = Instantiate(toSpawn, new Vector3(spawn.transform.position.x, spawn.transform.position.y), Quaternion.identity);
        fish.GetComponent<FishSwim>().OppSpawn = oppspawn;
        fish.GetComponent<FishSwim>().left = L2Side;
        fish.GetComponent<FishSwim>().depth = 2;
        fish.GetComponent<FishSwim>().speed = 3;
        fish.GetComponent<FishSwim>().fishType = fishtype;

        yield return new WaitForSeconds(5);

        StartCoroutine(Lane2());
    }

    IEnumerator Lane3()
    {
        GameObject fish;
        GameObject spawn;
        GameObject oppspawn;
        GameObject toSpawn = Tuna;

        int fishtype = Random.Range(1, 5);
        switch (fishtype)
        {
            case 1:
                toSpawn = Tuna;
                break;

            case 2:
                toSpawn = Salmon;
                break;

            case 3:
                toSpawn = Yellowtail;
                break;

            case 4:
                toSpawn = Eel;
                break;
        }

        if (L3Side)
        {
            spawn = L3L;
            oppspawn = L3R;
            L3Side = false;
        }
        else
        {
            spawn = L3R;
            oppspawn = L3L;
            L3Side = true;
        }
        fish = Instantiate(toSpawn, new Vector3(spawn.transform.position.x, spawn.transform.position.y), Quaternion.identity);
        fish.GetComponent<FishSwim>().OppSpawn = oppspawn;
        fish.GetComponent<FishSwim>().left = L3Side;
        fish.GetComponent<FishSwim>().depth = 2;
        fish.GetComponent<FishSwim>().speed = 3;
        fish.GetComponent<FishSwim>().fishType = fishtype;

        yield return new WaitForSeconds(5);

        StartCoroutine(Lane3());
    }

    IEnumerator Lane4()
    {
        GameObject fish;
        GameObject spawn;
        GameObject oppspawn;
        GameObject toSpawn = Tuna;

        int fishtype = Random.Range(1, 5);
        switch (fishtype)
        {
            case 1:
                toSpawn = Tuna;
                break;

            case 2:
                toSpawn = Salmon;
                break;

            case 3:
                toSpawn = Yellowtail;
                break;

            case 4:
                toSpawn = Eel;
                break;
        }

        if (L4Side)
        {
            spawn = L4L;
            oppspawn = L4R;
            L4Side = false;
        }
        else
        {
            spawn = L4R;
            oppspawn = L4L;
            L4Side = true;
        }
        fish = Instantiate(toSpawn, new Vector3(spawn.transform.position.x, spawn.transform.position.y), Quaternion.identity);
        fish.GetComponent<FishSwim>().OppSpawn = oppspawn;
        fish.GetComponent<FishSwim>().left = L4Side;
        fish.GetComponent<FishSwim>().depth = 3;
        fish.GetComponent<FishSwim>().speed = 5;
        fish.GetComponent<FishSwim>().fishType = fishtype;

        yield return new WaitForSeconds(5);

        StartCoroutine(Lane4());
    }

    IEnumerator Lane5()
    {
        GameObject fish;
        GameObject spawn;
        GameObject oppspawn;
        GameObject toSpawn = Tuna;

        int fishtype = Random.Range(0, 5);
        switch (fishtype)
        {
            case 0:
                toSpawn = SpicyTuna;
                break;

            case 1:
                toSpawn = Alaska;
                break;

            case 2:
                toSpawn = California;
                break;

            case 3:
                toSpawn = Caterpillar;
                break;

            case 4:
                toSpawn = Dynamite;
                break;
        }

        if (L5Side)
        {
            spawn = L5L;
            oppspawn = L5R;
            L5Side = false;
        }
        else
        {
            spawn = L5R;
            oppspawn = L5L;
            L5Side = true;
        }
        fish = Instantiate(Crab, new Vector3(spawn.transform.position.x, spawn.transform.position.y), Quaternion.identity);
        fish.GetComponent<FishSwim>().OppSpawn = oppspawn;
        fish.GetComponent<FishSwim>().left = L5Side;
        fish.GetComponent<FishSwim>().depth = 3;
        fish.GetComponent<FishSwim>().speed = 2;
        fish.GetComponent<FishSwim>().fishType = 5;

        yield return new WaitForSeconds(5);

        StartCoroutine(Lane5());
    }

    IEnumerator Countdown() {
        float curTime = 0;
        while (curTime < 120) {
            time.value -= Time.deltaTime;
            curTime += Time.deltaTime;
            yield return null;
        }
        if (curTime >= 120) {
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    
    }


}

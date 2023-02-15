using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : MonoBehaviour
{
    public enum Level { start, eagle, bee};
    public Level currentLevel = Level.start;

    // todo? enum for state playing, pause

    [SerializeField] List<LevelTrigger> levelTriggers;
    [SerializeField] Transform eagleStory;

    [Header("Menu Components")]
    [SerializeField] List<Transform> menuObjects = new List<Transform>();
    [SerializeField] Transform parentObjects;
    /*
    [SerializeField] Transform parentObject;
    [SerializeField] Transform bonfire;
    [SerializeField] Transform eagle;
    [SerializeField] Transform bee;
    [SerializeField] Transform tent;
    [SerializeField] Transform levelSign1;
    [SerializeField] Transform levelSign2;
    */

    // todo? a button to display menu in game

    StoryParts storyParts;
    

    private void Start()
    {
        storyParts = FindObjectOfType<StoryParts>();
        ChangeLevel(Level.start);
    }

    private void OnEnable()
    {
        foreach (LevelTrigger trigger in levelTriggers)
        {
            // connect onPressed with StartIntroSequence.
            trigger.onPressed.AddListener(ChangeLevel);
        }
    }

    private void OnDisable()
    {
        foreach (LevelTrigger trigger in levelTriggers)
        {
            // disconnect onPressed with StartIntroSequence.
            trigger.onPressed.RemoveAllListeners();
        }
    }

    /// <summary>
    /// Sequence to hide or display Menu Game Objects.
    /// </summary>
    /// <param name="show"></param>
    /// <returns></returns>
    private IEnumerator MenuSequence(bool show, float delay, List<Transform> exceptions)
    {
        yield return new WaitForSeconds(delay);

        float seconds = 0.01f;
        float add = 0.1f;
        foreach (Transform t in menuObjects)
        {
            Debug.Log("Working on: " + t.name);
            t.gameObject.SetActive(true);
            // if transform in exception - continue;
            if (IsInList(t, exceptions)) continue;
            
            MeshRenderer[] renderers = t.GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer renderer in renderers)
            {
                if (show)
                {
                    StartCoroutine(FadeInObject(renderer, 0.15f));
                }
                else
                {
                    StartCoroutine(FadeOutObject(renderer, 0.15f));
                }
                seconds += add;
                yield return new WaitForSeconds(seconds);
            } 
        }
        /*
        //todo Make exception for tent frame for eagle level.
        // Separate function in two
        // HideMenu()
        // ShowMenu()

        // todo? Make animation for each menu component.

        // Start after a short delay.
        yield return new WaitForSeconds(delay);
        float seconds = 0.01f;
        float add = 0.1f;
        foreach(Transform t in menuObjects)
        {
            float direction = 1; // -1 if hiding
            if (!show)
            {
                direction = -1;
                Animator animator = t.GetComponent<Animator>();
                if (animator != null)
                {
                    // hide
                    // Change animation state on replay animation on reverse
                    // Reverse Animation
                    // then set active
                    animator.SetFloat("animDirection", direction);
                    animator.SetTrigger("Repeat");
                }
            }
            yield return new WaitForSeconds(seconds);
            seconds += add;
        }

        /*
        if (!show)
        {
            yield return new WaitForSeconds(2.0f);
        }

        foreach(Transform t in menuObjects)
        {
            // to display
            t.gameObject.SetActive(show);
            yield return new WaitForSeconds(seconds);
            seconds += add;
        }*/
    }

    private IEnumerator HideMenu(float delay, List<Transform> exceptions)
    {
        
        yield return new WaitForSeconds(delay);
        float seconds = 0.01f;
        float add = 0.1f;
        foreach (Transform t in menuObjects)
        {
            // if transform in exception - continue;
            if (IsInList(t, exceptions)) continue;

            float direction = -1; // to reverse Animation.
            Animator animator = t.GetComponent<Animator>();
            if (animator != null)
            {
                // Repeats animation in reverse.
                animator.SetFloat("animDirection", direction);
                animator.SetTrigger("Repeat");
            }
            yield return new WaitForSeconds(seconds);
            seconds += add;
        }
        yield return new WaitForSeconds(5.0f);

        // after all animations finish playing.
        ShowAllMenuObjects(false);
        yield return null;
    }

    private IEnumerator ShowMenu(float delay)
    {
        yield return new WaitForSeconds(delay);
        float seconds = 0.01f;
        float add = 0.1f;
        foreach (Transform t in menuObjects)
        {
            // to display
            t.gameObject.SetActive(true);
            yield return new WaitForSeconds(seconds);
            seconds += add;
        }
    }

    private void ChangeLevel(Level level)
    {
        // Change Level
        Debug.Log("Changing Level");
        List<Transform> toIgnore = new List<Transform>();
        switch (level)
        {
            case Level.start:
                // initial state
                StopAllCoroutines();
                Debug.Log("Initial Game State");
                ShowAllMenuObjects(false);
                eagleStory.gameObject.SetActive(false);
                StartCoroutine(MenuSequence(true, 0.25f, null));
                currentLevel = Level.start;
                break;

            case Level.eagle:
                StopAllCoroutines();
                Debug.Log("Starting Eagle Level");
                storyParts.StartIntroSequence();
                eagleStory.gameObject.SetActive(true);

                toIgnore.Clear();
                toIgnore.Add(menuObjects[6]);
                StartCoroutine(MenuSequence(false, 0.05f, toIgnore));

                currentLevel = Level.eagle;
                break;

            case Level.bee:
                StopAllCoroutines();
                Debug.Log("Starting Bee Level");
                
                StartCoroutine(MenuSequence(false, 0.01f, null));

                currentLevel = Level.bee;
                break;
        } 
    }

    private void ShowAllMenuObjects(bool display)
    {
        //parentObjects.gameObject.SetActive(display);
        // Helper function.
        foreach(Transform t in menuObjects)
        {
            Debug.Log("Disabling: " + t.name);
            t.gameObject.SetActive(display);
        }
    }

    private bool IsInList(Transform transform, List<Transform> list)
    {
        if (list == null) return false;
        foreach(Transform t in list)
        {
            if (t == transform)
            {
                list.Remove(t);
                return true;
            }
                
        }
        return false;
    }

    private IEnumerator FadeInObject(Renderer renderer, float speed = 1f)
    {
        foreach(Material mat in renderer.materials)
        {
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0);
            while (mat.color.a < 1)
            {
                Color objectColor = mat.color;
                float fadeAmount = objectColor.a + (speed * Time.deltaTime);
                objectColor.a = fadeAmount;
                mat.color = objectColor;
                yield return null;
            }
        }
        yield return null;
    }

    private IEnumerator FadeOutObject(Renderer renderer, float speed = 1f)
    {
        foreach (Material mat in renderer.materials)
        {
            while (mat.color.a > 0)
            {
                Color objectColor = mat.color;
                float fadeAmount = objectColor.a - (speed * Time.deltaTime);
                objectColor.a = fadeAmount;
                mat.color = objectColor;
                yield return null;
            }
            renderer.gameObject.SetActive(false);
        }
    }
}

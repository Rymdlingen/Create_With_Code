using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    Button button;
    ParticleSystem stars;

    // Start is called before the first frame update
    void Start()
    {
        button = transform.GetComponent<Button>();
        stars = button.gameObject.transform.Find("Particle System").GetComponent<ParticleSystem>();
    }

    // Start the star particles when an button is selected.
    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        stars.Play();
    }

    // Stop the particle effect and remover the particles.
    void IDeselectHandler.OnDeselect(BaseEventData eventData)
    {
        stars.Stop();
        stars.Clear();
    }
}

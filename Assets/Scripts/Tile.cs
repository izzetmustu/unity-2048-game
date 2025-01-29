using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Tile : MonoBehaviour
{
    public TileState state {get; private set;}
    public TileCell cell {get; private set;}
    public int number {get; private set;}

    public bool locked {get; set;}

    private Image background;
    private TextMeshProUGUI text;

    private void Awake()
    {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetState(TileState state, int number)
    {
        this.state = state;
        this.number = number;
        background.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = number.ToString();
    }

    public void Spawn(TileCell cell)
    {
        if(this.cell != null)
        {
            this.cell.tile = null;
        }
        this.cell = cell;
        this.cell.tile = this;
        transform.position = cell.transform.position;
    }

    public void Move(TileCell cell)
    {
        if(this.cell != null)
        {
            this.cell.tile = null;
        }
        this.cell = cell;
        this.cell.tile = this;

        StartCoroutine(Animate(cell.transform.position));
    }
    public void Merge(TileCell cell)
    {
        if(this.cell != null)
        {
            this.cell.tile = null;
        }
        this.cell = null;
        cell.tile.locked = true;
        StartCoroutine(Animate(cell.transform.position, true));
    }

    private IEnumerator Animate(Vector3 targetPosition, bool merge = false)
    {
        float duration = 0.1f;
        float time = 0;
        Vector3 startPosition = transform.position;
        while(time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time/duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        if(merge)
        {
            Destroy(gameObject);
        }
    }
}

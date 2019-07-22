using UnityEngine;
using System.Collections;

public abstract class Moving : MonoBehaviour
{

    public float moveTime = 0.16f;
    public LayerMask blockingLayer;

    private BoxCollider2D m_boxCollider;
    private Rigidbody2D m_rigidbody;
    private float m_inverseMoveTime;

    protected virtual void Start ()
    {
        m_boxCollider = GetComponent<BoxCollider2D>();

        m_rigidbody = GetComponent<Rigidbody2D>();

        m_inverseMoveTime = 1f / moveTime;
    }

    public virtual bool Move (int xDir, int yDir)
    {
        RaycastHit2D hit;

        Vector2 startPos = transform.position;

        Vector2 endPos = startPos + new Vector2(xDir, yDir);

        m_boxCollider.enabled = false;

        hit = Physics2D.Linecast(startPos, endPos, blockingLayer);

        m_boxCollider.enabled = true;

        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(endPos));
            return true;
        }
        return false;
    }

    protected virtual IEnumerator SmoothMovement (Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPostion = Vector3.MoveTowards(m_rigidbody.position, end, m_inverseMoveTime * Time.deltaTime);

            m_rigidbody.MovePosition(newPostion);

            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }
    }
}

using System.Collections;
using UnityEngine;

// Impose la présence des composants nécessaires sur le même GameObject :
// - Rigidbody2D : déplacement physique;
// - Collider2D : détection des contacts;
// - SpriteRenderer : affichage du sprite.
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class EnnemiMobile : MonoBehaviour
{
    // Liste des modes de déplacement disponibles dans l'Inspector.
    public enum TypeDeplacement
    {
        Patrouille, // Aller-retour entre deux points.
        Sinusoidal // Déplacement vers une cible qui suit une vague.
    }

    [Header("Patrouille")]

    // Mode utilisé lorsque le joueur n'est pas détecté.
    [SerializeField]
    private TypeDeplacement typeDeplacement = TypeDeplacement.Patrouille;

    // Repères définissant les extrémités du trajet.
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    // Vitesse de patrouille en unités Unity par seconde.
    [SerializeField, Min(0.1f)]
    private float vitessePatrouille = 1.8f;

    // Amplitude verticale de la vague, en unités Unity.
    [SerializeField, Min(0.1f)]
    private float hauteurVague = 1.1f;

    // Contrôle la vitesse de progression de la vague
    // et des allers-retours entre A et B.
    [SerializeField, Min(0.1f)]
    private float frequenceVague = 1.4f;

    [Header("Poursuite")]

    // Position du joueur à suivre.
    [SerializeField] private Transform joueur;

    // Distance maximale à laquelle l'ennemi détecte le joueur.
    [SerializeField, Min(0.5f)]
    private float rayonDetection = 3.5f;

    // Vitesse utilisée pendant la poursuite.
    [SerializeField, Min(0.1f)]
    private float vitessePoursuite = 2.8f;

    [Header("Mort")]

    // Durée de l'animation de mort avant que l'ennemi devienne ramassable.
    [SerializeField, Min(0.1f)]
    private float dureeMort = 0.6f;

    // Références aux composants de l'ennemi.
    private Rigidbody2D corps;
    private SpriteRenderer rendu;

    // Composants facultatifs : GetComponent retourne null s'ils sont absents.
    private Animator animateur;
    private Collectable collectable;

    // Point actuellement visé pendant la patrouille.
    private Transform ciblePatrouille;

    // Compteur utilisé pour calculer le mouvement ondulé.
    private float progressionVague;

    // Indique que l'ennemi a été tué.
    private bool mort;

    // Effet visuel d'attaque, s'il existe sur cet objet.

    private void Awake()
    {
        // Récupère les composants présents sur le même GameObject.
        corps = GetComponent<Rigidbody2D>();
        rendu = GetComponent<SpriteRenderer>();

        // Empêche la gravité de faire tomber l'ennemi.
        corps.gravityScale = 0f;

        // Empêche les interactions physiques de le faire tourner.
        corps.freezeRotation = true;

        animateur = GetComponent<Animator>();

        // Le Collectable reste désactivé tant que l'ennemi est vivant.
        collectable = GetComponent<Collectable>();
        if (collectable != null)
            collectable.enabled = false;
    }

    private void Start()
    {
        // Choisit B comme première destination si B est assigné.
        // Sinon, utilise A.
        ciblePatrouille = pointB != null ? pointB : pointA;
    }

    // Appelée à intervalles fixes pour gérer la physique.
    private void FixedUpdate()
    {
        // Un ennemi mort reste immobile.
        if (mort)
        {
            corps.linearVelocity = Vector2.zero;
            return;
        }

        // Le joueur est détecté si sa référence existe ET
        // si sa distance à l'ennemi ne dépasse pas le rayon de détection.
        bool joueurDetecte = joueur != null &&
            Vector2.Distance(corps.position, joueur.position) <= rayonDetection;

        // La poursuite est prioritaire sur les autres déplacements.
        if (joueurDetecte)
            PoursuivreJoueur();
        else if (typeDeplacement == TypeDeplacement.Sinusoidal)
            DeplacementSinusoidal();
        else
            DeplacementPatrouille();
    }

    private void PoursuivreJoueur()
    {
        Vector2 direction =
            ((Vector2)joueur.position - corps.position).normalized;

        AppliquerVitesse(direction, vitessePoursuite);
    }

    private void DeplacementPatrouille()
    {
        // Sans destination, aucun nouveau déplacement n'est calculé.
        if (ciblePatrouille == null) return;

        Vector2 direction =
            ((Vector2)ciblePatrouille.position - corps.position).normalized;

        AppliquerVitesse(direction, vitessePatrouille);

        // Quand l'ennemi arrive près de sa destination,
        // change de point cible pour effectuer un aller-retour.
        if (Vector2.Distance(corps.position, ciblePatrouille.position) < 0.2f)
            ciblePatrouille = ciblePatrouille == pointA ? pointB : pointA;
    }

    private void DeplacementSinusoidal()
    {
        // Ce mode nécessite les deux points.
        if (pointA == null || pointB == null) return;

        progressionVague += Time.fixedDeltaTime * frequenceVague;

        float allerRetour = Mathf.PingPong(progressionVague, 1f);

        Vector2 baseTrajet =
            Vector2.Lerp(pointA.position, pointB.position, allerRetour);

        Vector2 cibleVague = baseTrajet + Vector2.up *
            (Mathf.Sin(progressionVague * Mathf.PI * 2f) * hauteurVague);

        Vector2 direction = (cibleVague - corps.position).normalized;

        // Utilise une vitesse supérieure de 15 % à celle de la patrouille.
        AppliquerVitesse(direction, vitessePatrouille * 1.15f);
    }

    // Centralise le déplacement et l'orientation visuelle.
    private void AppliquerVitesse(Vector2 direction, float vitesse)
    {
        corps.linearVelocity = direction * vitesse;

        // Retourne le sprite horizontalement lorsque l'ennemi va à gauche.
        // Suppose que le dessin d'origine regarde vers la droite.
        if (Mathf.Abs(direction.x) > 0.05f)
            rendu.flipX = direction.x < 0f;
    }

    // Appelée par le script d'attaque du joueur.
    public void Mourir()
    {
        // Évite de mourir deux fois.
        if (mort) return;

        mort = true;
        corps.linearVelocity = Vector2.zero;

        // Déclenche l'animation de mort si un Animator est présent.
        animateur?.SetTrigger("mort");

        StartCoroutine(DevenirCollectable());
    }

    private IEnumerator DevenirCollectable()
    {
        // Laisse jouer l'animation de mort.
        yield return new WaitForSeconds(dureeMort);

        // Active le Collectable : le joueur peut maintenant le ramasser.
        if (collectable != null)
            collectable.enabled = true;
    }

    // Appelée pendant que l'autre Collider2D reste dans la zone Trigger.
    private void OnTriggerStay2D(Collider2D autre)
    {
        // Ignore le contact si l'ennemi est mort, si l'objet n'a pas
        // le tag Player, ou si le joueur est déjà en train de mourir.
        if (mort || !autre.CompareTag("Player")) return;
        if (DeathManager.Instance != null && DeathManager.Instance.IsDying) return;


        // Un seul contact suffit à tuer le joueur.
        DeathManager.Instance?.KillPlayer(autre.gameObject);
    }

    // Dessine des repères dans la vue Scene lorsque l'objet est sélectionné.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // Visualise le rayon de détection du joueur.
        Gizmos.DrawWireSphere(transform.position, rayonDetection);

        // Visualise le segment entre les deux points de patrouille.
        if (pointA != null && pointB != null)
            Gizmos.DrawLine(pointA.position, pointB.position);
    }
}
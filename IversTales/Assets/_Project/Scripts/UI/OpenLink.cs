using UnityEngine;

/// <summary>Opens an external web page (social networks, feedback form...) from a UI button.</summary>
public class OpenLink : MonoBehaviour
{
    public void OpenLinks(string link)
    {
        if (string.IsNullOrWhiteSpace(link)) return;
        if (!link.StartsWith("https://") && !link.StartsWith("http://"))
        {
            Debug.LogWarning($"{nameof(OpenLink)}: refusing to open '{link}' (only http/https links are allowed).", this);
            return;
        }
        Application.OpenURL(link);
    }
}

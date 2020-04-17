using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SamOatesGames.Systems
{
    public class LanguagePhraseResolver : MonoBehaviour
    {
        /// <summary>
        /// The text component to be resolved.
        /// </summary>
        private Text m_textComponent;
        private TMP_Text m_tmpTextComponent;

        /// <summary>
        /// The phrase key this resolver should resolve.
        /// </summary>
        private string m_phraseKey;

        /// <summary>
        /// 
        /// </summary>
        private bool m_setup;

        /// <summary>
        /// Register this resolve, and store the text component.
        /// </summary>
        private void Start()
        {
            if (m_setup)
            {
                // Already setup
                return;
            }

            if (!DiscoverTextComponent())
            {
                return;
            }

            m_phraseKey = GetText();

            var languageSystem = LanguageSystem.GetInstance();
            if (languageSystem != null)
            {
                languageSystem.RegisterResolver(this);
            }

            m_setup = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool DiscoverTextComponent()
        {
            if (m_textComponent != null || m_tmpTextComponent != null)
            {
                return true;
            }

            m_textComponent = GetComponent<Text>();
            if (m_textComponent != null)
            {
                return true;
            }

            m_tmpTextComponent = GetComponent<TMP_Text>();
            if (m_tmpTextComponent != null)
            {
                return true;
            }

            Debug.LogErrorFormat("'{0}' attempted to run a resolver on a game object without a text component", name);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetText()
        {
            if (m_textComponent != null)
            {
                return m_textComponent.text;
            }

            if (m_tmpTextComponent != null)
            {
                return m_tmpTextComponent.text;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void SetText(string value)
        {
            if (m_textComponent != null)
            {
                m_textComponent.text = value;
                return;
            }

            if (m_tmpTextComponent != null)
            {
                m_tmpTextComponent.text = value;
                return;
            }

            Debug.LogErrorFormat("Trying to set the text on '{0}' when we don't have a valid text component.", name);
        }

        /// <summary>
        /// Called when the script is destroyed.
        /// Unregister the resolver.
        /// </summary>
        private void OnDestroy()
        {
            if (LanguageSystem.Exists())
            {
                var languageSystem = LanguageSystem.GetInstance();
                languageSystem.RemoveResolver(this);
            }
        }

        /// <summary>
        /// Set the phrase key for this resolver and re-resolve the phrase.
        /// </summary>
        /// <param name="phraseKey"></param>
        public void SetPhraseKey(string phraseKey)
        {
            if (!DiscoverTextComponent())
            {
                return;
            }

            m_phraseKey = phraseKey;
            SetText(phraseKey);

            var languageSystem = LanguageSystem.GetInstance();
            if (languageSystem != null)
            {
                Resolve(languageSystem);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            if (!m_setup)
            {
                return;
            }

            SetText(m_phraseKey);
        }

        /// <summary>
        /// Resolve this resolver
        /// </summary>
        /// <param name="languageSystem">The language system to use when resolving</param>
        public void Resolve(LanguageSystem languageSystem)
        {
            if (string.IsNullOrEmpty(m_phraseKey))
            {
                return;
            }

            var phrases = Regex.Matches(m_phraseKey, @"\[\[(?<phrase>\w+)\]\]");
            var vars = Regex.Matches(m_phraseKey, @"\{\{(?<var>.*)\}\}");

            // If no phrases or variables are found, just resolve the entire key
            if (phrases.Count == 0 && vars.Count == 0)
            {
                SetText(languageSystem.ResolvePhrase(m_phraseKey));
                if (string.IsNullOrEmpty(GetText()))
                {
                    SetText(m_phraseKey);
                    Debug.LogWarningFormat("Failed to find language phrase '{0}' requested by '{1}'.", m_phraseKey, name);
                }
            }
            else
            {
                var fullText = m_phraseKey;

                // We have sub-phrases, resolve one at a time.
                foreach (var phraseMatch in phrases.Cast<Match>())
                {
                    var phrase = phraseMatch.Groups["phrase"].Value;
                    var resolvedPhrase = languageSystem.ResolvePhrase(phrase);
                    fullText = fullText.Replace(string.Format("[[{0}]]", phrase), resolvedPhrase);
                }

                var variableResolver = VariableResolvingSystem.GetInstance();
                if (variableResolver != null)
                {
                    vars = Regex.Matches(fullText, @"\{\{(?<var>.*?)\}\}");

                    // We have sub-variable, resolve one at a time.
                    foreach (var varMatch in vars.Cast<Match>())
                    {
                        var varValue = varMatch.Groups["var"].Value;
                        var resolvedVar = variableResolver.ResolveVariable(varValue);
                        fullText = fullText.Replace(string.Format("{{{{{0}}}}}", varValue), resolvedVar);
                    }
                }

                SetText(fullText);
            }
        }
    }
}

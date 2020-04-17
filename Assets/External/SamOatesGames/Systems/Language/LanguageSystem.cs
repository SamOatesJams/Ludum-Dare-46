using System.Collections.Generic;
using System.Linq;
using SamOatesGames.Systems.Core;
using UnityEngine;

namespace SamOatesGames.Systems
{
    public class LanguageSystem : UnitySingleton<LanguageSystem>
    {
        [Tooltip("The text files to parse the language data from.")]
        public TextAsset[] LaunguageFiles;

        /// <summary>
        /// All loaded phrases.
        /// Language -> {Key, Phrase}
        /// </summary>
        private readonly Dictionary<string, List<KeyValuePair<string, string>>> m_phrases = new Dictionary<string, List<KeyValuePair<string, string>>>();

        /// <summary>
        /// All supported languages
        /// </summary>
        private readonly HashSet<string> m_supportLanguages = new HashSet<string>();

        /// <summary>
        /// All registered phrase resolvers
        /// </summary>
        private readonly HashSet<LanguagePhraseResolver> m_phraseResolvers = new HashSet<LanguagePhraseResolver>();

        /// <summary>
        /// Get all support languages
        /// </summary>
        public IReadOnlyList<string> SupportedLanguages
        {
            get { return m_supportLanguages.ToArray(); }
        }

        /// <summary>
        /// 
        /// </summary>
        private string m_selectedLanguage;
        public string SelectedLanguage
        {
            get { return m_selectedLanguage == null ? SupportedLanguages.First() : m_selectedLanguage; }
            set
            {
                if (!m_supportLanguages.Contains(value))
                {
                    Debug.LogErrorFormat("Can not set selected language to '{0}' as it isn't in the list of supported languages.", value);
                    return;
                }

                if (m_selectedLanguage != value)
                {
                    m_selectedLanguage = value;
                    ResolveAllRegisteredBehaviours();
                }
            }
        }

        /// <summary>
        /// Parse the language file and setup all language properties
        /// </summary>
        protected override void Awake ()
        {
            base.Awake();

            if (LaunguageFiles == null)
            {
                Debug.LogError("[LanguageSystem] The 'LaunguageFiles' text assets has not been set.");
                return;
            }

            foreach (var languageFile in LaunguageFiles)
            {
                ParseLanguageContents(languageFile);
            }
        }

        /// <summary>
        /// Parse a CSV file
        /// </summary>
        /// <param name="textAsset">The contents to parse</param>
        private void ParseLanguageContents(TextAsset textAsset)
        {
            string[][] contents = CSVReader.ParseFile(textAsset.text);
        
            if (contents.Length < 1)
            {
                // Nothing to parse
                return;
            }

            var sheetName = textAsset.name.Split(' ').LastOrDefault();

            // First row lists all languages
            var supportedLanguagesRaw = contents.First();
            
            // Load all languages from the first row.
            // Skip the first cell as the first column is the phrase key column.
            for (int languageIndex = 1; languageIndex < supportedLanguagesRaw.Length; ++languageIndex)
            {
                var language = supportedLanguagesRaw[languageIndex].Trim();
                if (m_supportLanguages.Add(language))
                {
                    m_phrases.Add(language, new List<KeyValuePair<string, string>>());
                }
            }

            // Load each row.
            // The first cell is the phrase ID.
            // All other rows are per language values.
            foreach (var row in contents.Skip(1))
            {
                var phraseId = sheetName + "_" + row.First();

                var languageIndex = 1;
                foreach (var currentphrase in row.Skip(1))
                {
                    m_phrases[supportedLanguagesRaw[languageIndex++].Trim()].Add(new KeyValuePair<string, string>(phraseId, currentphrase));
                }
            }
        }

        /// <summary>
        /// Resolve a phrase ID for a given language
        /// </summary>
        /// <param name="language">The language to resolve</param>
        /// <param name="phraseId">The phrase to resolve</param>
        /// <returns>The language specific phrase of 'INVALID_PHRASE' if it does not exist.</returns>
        private string ResolvePhrase(string language, string phraseId)
        {
            if (!m_phrases.TryGetValue(language, out var phrases))
            {
                return string.Empty;
            }

            var phraseEntry = phrases.Find(x => x.Key == phraseId);
            return phraseEntry.Value ?? string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="language"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public IList<KeyValuePair<string, string>> GetPhrasesWithPrefix(string language, string prefix)
        {
            if (!m_phrases.TryGetValue(language, out var phrases))
            {
                return new KeyValuePair<string, string>[0];
            }

            var matching = new List<KeyValuePair<string, string>>();
            foreach (var phraseEntry in phrases)
            {
                if (phraseEntry.Key.StartsWith(prefix))
                {
                    matching.Add(phraseEntry);
                }
            }

            return matching;
        }

        /// <summary>
        /// Resolve a phrase ID for the active language
        /// </summary>
        /// <param name="phraseId">The phrase to resolve</param>
        /// <returns>The language specific phrase of 'INVALID_PHRASE' if it does not exist.</returns>
        public string ResolvePhrase(string phraseId)
        {     
            return ResolvePhrase(SelectedLanguage, phraseId);
        }

        /// <summary>
        /// Register a resolver.
        /// Resolvers will get auto resolved when the language is changed.
        /// </summary>
        /// <param name="resolver"></param>
        public void RegisterResolver(LanguagePhraseResolver resolver)
        {
            if (!m_phraseResolvers.Contains(resolver))
            {
                m_phraseResolvers.Add(resolver);
                resolver.Resolve(this);
            }
        }

        /// <summary>
        /// Remove a phrase resolver from the system.
        /// </summary>
        /// <param name="resolver"></param>
        public void RemoveResolver(LanguagePhraseResolver resolver)
        {
            if (m_phraseResolvers.Contains(resolver))
            {
                m_phraseResolvers.Remove(resolver);
            }
        }

        /// <summary>
        /// Resolve all registered resolvers.
        /// Normally happens when a language has changed.
        /// </summary>
        public void ResolveAllRegisteredBehaviours()
        {
            foreach (var resolver in m_phraseResolvers)
            {
                resolver.Resolve(this);
            }
        }
    }
}

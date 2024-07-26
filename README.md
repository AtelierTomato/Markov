# AtelierTomato.Markov
This project offers Markov Chain logic, data structures, storage solutions, parsers, and renderers as libraries that can be used modularly.

# Guide
This guide will offer a basic overview on how to set up an AtelierTomato.Markov implementation.

## Parsers
The first step is to choose a parser or multiple parsers. The default `SentenceParser`, which can be found in `AtelierTomato.Markov.Core` can be used if you are handling a platform that has no markdown. It will separate punctuation and some contractions, focused specifically on the English language, this improves Markov chaining by allowing for more matches to occur where they otherwise would not. For instance, contractions like `don't` are separated into `don 't` when put in storage. This is especially useful for possessive possessive nouns like `Kelsey's` or `Alice's`, since they are separated out into `Kelsey 's` and `Alice 's`, the `'s` can be matched to any other instance of `'s`, improving sentence generation output. It is possible to skip using parsers, or to use your own, if you would prefer to though.
Parsers for individual services can be found as packages named `AtelierTomato.Markov.Service.X`. All service-specific parsers extend off of the default `SentenceParser`.
A current list of all parsers we have and what they offer is below:
- `SentenceParser`: Parses the English language in order to provide more efficient Markov chaining. Deletes links. Separates input into multiple sentences determined by punctuation marks or line breaks. Removes sentences that are smaller than a configurable MinimumInputLength.
- `DiscordSentenceParser`: Removes all Discord markdown. Removes or replaces certain bot prefixes (configurable). Removes sentences that begin with certain bot prefixes (configurable). Stores custom emojis in our own format (looks like this: `e:EmojiName:`). Completely ignores all text inside of code blocks. Removes discord server links, even those that do not begin with `https://`.

## OID building
WIP, need to add classes for this.

## Storage
To use AtelierTomato.Markov, you must use a storage implementation that implements our model for `Sentence`, found in AtelierTomato.Markov.Model. We offer the interface `ISentenceAccess` that you may create implementations of if we do not have a storage solution that you prefer already offered. However, you must use something that extends from `ISentenceAccess`, and thus uses our model, as the Markov chain logic requires it. The base `AtelierTomato.Markov.Storage` package includes `InMemorySentenceAccess` as well as interfaces for Sentence and WordStatistic (more on this later) access. All other interface implementations are found in packages `AtelierTomato.Markov.Storage.X`.
Current offered storage solutions:
- `SqliteSentenceAccess`
- `InMemorySentenceAccess` NOTE: Should only be used for testing, will likely be removed in the future. Stores `Sentence`s in RAM, and they will be lost when the program is terminated.
If you would like to use keyword generation (explained below), then the text that you store as a `Sentence` should also be stored using `IWordStatisticAccess.WriteWordStatisticsFromString`.

## Keyword generation
This step can be skipped completely, however, keywords are powerful for improving sentence generation by allowing user input to nudge the chain to be "aware" of what the user is prompting the chain with. We offer the type `WordStatistic`, as well as the storage for it in `IWordStatisticAccess` and classes extending off of it. A `WordStatistic` is a simple measurement of how frequently a word appears. The current keyword generation logic, found in `KeywordProvider`, simply looks for the least common `WordStatistic` above a certain configurable `MinimumAppearanceForKeyword` in a given string. You must pass in a pre-parsed string if you are using the `SentenceParser`. There is also a configurable option to ignore certain words from being used as keywords. The function `KeywordProvider.Find()` will return a single word as a string that can then be used as the `keyword` in the generation step. Its functionality will be explained there.

## Generation
If you do not want to use our provided keyword logic, which will be explained below, all you need to do is make an instance of `MarkovChain`, which must be provided options and an `ISentenceAccess` implementation, and run `MarkovChain.Generate()`. You must provide a `SentenceFilter` (found in `AtelierTomato.Markov.Model`), though this can be null. Sentence filtering, keywords, and first words can all be used in generation as parameters to `Generate()`. They are explained below:
- `filter`: Filters are of class `SentenceFilter`. This class includes the properties `OID` and `Author`, the same as that of the `Sentence`, these can be used to filter at any level, such as requiring a `Sentence` from the database to be from a certain Discord server or channel, or from a specific user.
- `keyword`: You can use this as a whitelist for `Sentence`s when querying, the chain will first look for those that match the `prevList` that include that word, if it fails to find any, then it will fallback to searching everything else (restricted by `filter` still, for both).
- `firstWord`: This will force the chain to start with a certain word, even if the word does not exist in the database (this will cause it to just output that word).

## Rendering
If you are using one of our `SentenceParser`s, you must use one of our `SentenceRenderer`s, unless you are fine having output have random spaces in it. Similar to the parsers, the base `SentenceRenderer` is extended by all service-specific renderers, and can be used to render things to plain text, it is found in `AtelierTomato.Markov.Core`. Service-specific renderers are found in packages `AtelierTomato.Markov.Service.X`. All renderers are compatible with all parsers, if you want to parse input with one parser on one platform and render it with another on antoher platform, you can with no difficulties or oddities.
A current list of all renderers we have and what they offer is below:
- `SentenceRenderer`: Renders separated punctuation and contractions to be connected again. Renders our stored custom emoji format into the name of the emoji.
- `DiscordSentenceRenderer`: Escapes all characters not found in 0-9a-zA-Z. Attempts to render our internal emoji format into a custom emoji that the implementation has access to that matches the name of the emoji, first checking the current Discord server and then all Discord servers it has access to.

## You're done!
That is the full extent of what is necessary to generate sentences with this project.

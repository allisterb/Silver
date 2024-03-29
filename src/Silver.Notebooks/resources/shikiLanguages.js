var boogieGrammar = {
	"uuid": "bf0e606e-362a-11ed-a261-0242ac120002",
    "name": "boogie",
    "patterns": [
        {
            "include": "#constants"
        },
        {
            "include": "#keywords"
        },
        {
            "include": "#strings"
        },
        {
            "include": "#comments"
        },
        {
            "name": "string.other.attribute.boogie",
            "begin": "{\\s*:",
            "end": "}",
            "patterns": [
                {
                    "include": "$self"
                }
            ]
        },
        {
            "name": "entity.name.block.boogie",
            "match": "(\\s*([^: ]+):(\n|\r))"
        },
        {
            "name": "punctuation.terminator.boogie",
            "match": ";"
        }
    ],
    "repository": {
        "constants": {
            "patterns": [
                {
                    "name": "constant.language.boolean.boogie",
                    "match": "(?<![\\w$.])(true|false)(?![\\w$.])"
                },
                {
                    "name": "constant.numeric.integer.boogie",
                    "match": "(?<![\\w$.])[0-9]+(?![\\w$.])"
                },
                {
                    "name": "constant.numeric.bitvector.boogie",
                    "match": "(?<![\\w$.])[0-9]+bv[0-9]+(?![\\w$.])"
                }
            ]
        },
        "keywords": {
            "patterns": [
                {
                    "name": "storage.type.declaration.boogie",
                    "match": "\\b(axiom|const|function|implementation|procedure|type|var)\\b"
                },
                {
                    "name": "storage.modifier.boogie",
                    "match": "(?<![\\w$.])(complete|extends|finite|free|unique|where)(?![\\w$.])"
                },
                {
                    "name": "keyword.other.specification.boogie",
                    "match": "\\b(requires|ensures|modifies|returns|invariant)\\b"
                },
                {
                    "name": "keyword.control.boogie",
                    "match": "\\b(break|call|async|par|while|goto|return)\\b"
                },
                {
                    "name": "keyword.control.conditional.boogie",
                    "match": "\\b(if|then|else)\\b"
                },
                {
                    "name": "keyword.control.statement.boogie",
                    "match": "\\b(assert|assume|havoc|yield)\\b"
                },
                {
                    "name": "keyword.operator.assignment.boogie",
                    "match": "(:=)"
                },
                {
                    "name": "keyword.other.old.boogie",
                    "match": "\\bold\\b"
                },
                {
                    "name": "keyword.other.logical.quantifier.boogie",
                    "match": "\\b(forall|exists|lambda)\\b"
                },
                {
                    "name": "keyword.operator.logical.boogie",
                    "match": "::"
                },
                {
                    "name": "keyword.operator.logical.unary.boogie",
                    "match": "!"
                },
                {
                    "name": "keyword.operator.logical.binary.boogie",
                    "match": "<==>|==>|<==|&&|\\|\\|"
                },
                {
                    "name": "keyword.operator.comparison.boogie",
                    "match": "==|!=|<=|>=|<:|<|>"
                }
            ]
        },
        "strings": {
            "name": "string.quoted.double.boogie",
            "begin": "\"",
            "end": "\"",
            "patterns": [
                {
                    "name": "constant.character.escape.boogie",
                    "match": "\\\\."
                }
            ]
        },
        "comments": {
            "patterns": [
                {
                    "name": "comment.line.double-slash.boogie",
                    "match": "(//).*$\\n?"
                },
                {
                    "name": "comment.block.boogie",
                    "begin": "/\\*",
                    "end": "\\*/"
                }
            ]
        }
    },
    "scopeName": "source.boogie"
}

var highlighter;
var renderer;
var boogieLanguage = {
  id: "boogie",
  scopeName: 'source.boogie',
  grammar: boogieGrammar,
  aliases: ["boogie"],
  embeddedLangs: ["boogie"]
};
if (shiki === undefined) {
    console.error("Shiki library is not loaded yet! Run the notebook init script.")
}
else {
    shiki
        .getSVGRenderer({
            fontFamily: "Fira Code",
            fontSize: 14
        })
        .then((i) => { renderer = i; });

    highlighter = shiki.getHighlighter({
        themes: ["github-light", "light-plus"],
        langs: [boogieLanguage]
    })
    .then((i) => {
        highlighter = i;
    });
}
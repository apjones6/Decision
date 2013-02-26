Decision project readme
=======================

Objectives:
 - Robust expression based policy execution
 - Custom XML configuration for policy and expression definition, which then leverages castle windsor for components
 - Environment system which supplies details based on all policies used in expression (and their implicit/directly requested details)
 - Reduce the infrastructure overhead (custom decision providers, separated policy providers)
 - ~~Support a more full set of operators in expressions (not, xor) as well as aliases for known operators (&, ., AND for logical and operations)~~
 - ~~Support whitespace in expressions robustly~~
 - Cache policy results as part of a complete expression to avoid repeated execution
 - Simplify DecisionContext but also reuse on request and resolution#
 - Allow parameters provided to policies via xml config, to support policy reuse (for example a single usertype policy reused for IsAdmin, IsLearner etc)
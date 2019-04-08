;; 9.0 Lists diagrammed
;; A pair of address-boxes is called a “cons cell” or “dotted pair”.

;; (setq bouquet '(rose violet buttercup))

;; bouquet
;;      |
;;      |    ___ ___      ___ ___      ___ ___
;;      --> |___|___|--> |___|___|--> |___|___|--> nil
;;            |            |            |
;;            |            |            |
;;             --> rose     --> violet   --> buttercup

;; (setq flowers (cdr bouquet))

;; bouquet        flowers
;;   |              |
;;   |     ___ ___  |     ___ ___      ___ ___
;;    --> |   |   |  --> |   |   |    |   |   |
;;        |___|___|----> |___|___|--> |___|___|--> nil
;;          |              |            |
;;          |              |            |
;;           --> rose       --> violet   --> buttercup

;; (setq bouquet (cons 'lily bouquet))

;; bouquet                       flowers
;;   |                             |
;;   |     ___ ___        ___ ___  |     ___ ___       ___ ___
;;    --> |   |   |      |   |   |  --> |   |   |     |   |   |
;;        |___|___|----> |___|___|----> |___|___|---->|___|___|--> nil
;;          |              |              |             |
;;          |              |              |             |
;;           --> lily      --> rose       --> violet    --> buttercup


;; 9.1 Symbols as a Chest of Drawers

;;     Chest of Drawers            Contents of Drawers
;;     __   o0O0o   __
;;   /                 \
;;  ---------------------
;; |    directions to    |            [map to]
;; |     symbol name     |             bouquet
;; |                     |
;; +---------------------+
;; |    directions to    |
;; |  symbol definition  |             [none]
;; |                     |
;; +---------------------+
;; |    directions to    |            [map to]
;; |    variable value   |             (rose violet buttercup)
;; |                     |
;; +---------------------+
;; |    directions to    |
;; |    property list    |             [not described here]
;; |                     |
;; +---------------------+
;; |/                   \|


;; 9.2 Exercise
;; Set ‘flowers’ to ‘violet’ and ‘buttercup’.  Cons two more flowers on to
;; this list and set this new list to ‘more-flowers’.  Set the CAR of
;; ‘flowers’ to a fish.  What does the ‘more-flowers’ list now contain?

(setq flowers '(violet buttercup))
(setq more-flowers (cons 'rose flowers))
(setq more-flowers (cons 'lily more-flowers))
(setcar flowers 'fish)
more-flowers

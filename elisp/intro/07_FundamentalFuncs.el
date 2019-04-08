;; 7.1 ‘car’ and ‘cdr’
;; CAR -> first
(car '(rose violet daisy buttercup))
;; CDR -> rest
(cdr '(rose violet daisy buttercup))


;; 7.2 ‘cons’
;; The ‘cons’ function constructs lists
(cons 'pine '(fir oak maple))

;; Build a list
;; ‘cons’ must have a list to attach to. You cannot sbtart from absolutely
;; nothing.
(cons 'buttercup ())
(cons 'daisy '(buttercup))
(cons 'violet '(daisy buttercup))
(cons 'rose '(violet daisy buttercup))

;; 7.2.1 Find the Length of a List: ‘length’
(length ())
(length '(buttercup))
(length '(daisy buttercup))
(length (cons 'violet '(daisy buttercup)))


;; 7.3 ‘nthcdr’
;; take the CDR of a list repeatedly
(cdr '(oak maple))
(cdr '(maple))
(cdr 'nil)
(cdr ())

(cdr (cdr '(pine fir oak maple)))
(nthcdr 2 '(pine fir oak maple))


;; 7.4 ‘nth’
(nth 0 '("one" "two" "three"))
(nth 1 '("one" "two" "three"))


;; 7.5 ‘setcar’
;; They actually change the original list, unlike ‘car’ and ‘cdr’
;; which leave the original list as it was

(setq animals '(antelope giraffe lion tiger))
animals

(setcar animals 'hippopotamus)
animals


;; 7.6 ‘setcdr’
(setq domesticated-animals '(horse cow sheep goat))
domesticated-animals

(setcdr domesticated-animals '(cat dog))
domesticated-animals


;; 7.7 Exercise
;; Construct a list of four birds by evaluating several expressions
;; with ‘cons’.  Find out what happens when you ‘cons’ a list onto
;; itself.  Replace the first element of the list of four birds with a
;; fish.  Replace the rest of that list with a list of other fish
(setq birds (cons 'canay '(duck parrot owl)))
(setcar birds 'tuna)
birds
(setcdr birds '(goldfish salmon eel))
birds

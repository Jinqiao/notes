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

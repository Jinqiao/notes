;; 11.1 ‘while’

(while TRUE-OR-FALSE-TEST
  BODY...)

;; The value returned by evaluating a ‘while’ is the value of the
;; true-or-false-test. A ‘while’ expression that evaluates
;; successfully never returns a true value!


;; To loop through a list
(while LIST
  BODY...
  SET-LIST-TO-CDR-OF-LIST)

;; Example, copy and eval in *scrach*
(setq animals '(gazelle giraffe lion tiger))

(defun print-elements-of-list (list)
  "Print each element of LIST on a line of its own."
  (while list
    (print (car list))
    (setq list (cdr list))))

(print-elements-of-list animals)


;; for-loop
SET-COUNT-TO-INITIAL-VALUE
(while (< count desired-number)         ; true-or-false-test
  BODY...
  (setq count (1+ count)))              ; incrementer

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


;; 11.2 Save your time: ‘dolist’ and ‘dotimes’
(dolist (VAR LIST [RESULT]) BODY...)
;; Evaluate BODY with VAR bound to each car from LIST, in turn.
;; Then evaluate RESULT to get return value, default nil.

(dotimes (VAR COUNT [RESULT]) BODY...)
;; Evaluate BODY with VAR bound to successive integers running from 0
;; to COUNT, [0, COUNT) Then evaluate RESULT to get the return value
;; (nil if RESULT is omitted).


;; 11.3 Recursion
(defun NAME-OF-RECURSIVE-FUNCTION (ARGUMENT-LIST)
       "DOCUMENTATION..."
       (if DO-AGAIN-TEST
         BODY...
         (NAME-OF-RECURSIVE-FUNCTION
          NEXT-STEP-EXPRESSION)))

; example
(setq animals '(gazelle giraffe lion tiger))
(defun print-elements-recursively (list)
  "Print each element of LIST on a line of its own.
     Uses recursion."
  (when list                        ; do-again-test
    (print (car list))              ; body
    (print-elements-recursively     ; recursive call
     (cdr list))))                  ; next-step-expression
(print-elements-recursively animals)

;; 11.3.5 Recursion Example Using ‘cond’
(defun triangle-using-cond (number)
  (cond ((<= number 0) 0)
        ((= number 1) 1)
        ((> number 1)
         (+ number (triangle-using-cond (1- number))))))

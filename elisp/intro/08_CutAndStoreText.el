;; 8.1 ‘zap-to-char’
(defun zap-to-char (arg char)
  "Kill up to and including ARG'th occurrence of CHAR.
     Case is ignored if `case-fold-search' is non-nil in the current buffer.
     Goes backward if ARG is negative; error if CHAR not found."
  (interactive "p\ncZap to char: ")
  (if (char-table-p translation-table-for-input)
      (setq char (or (aref translation-table-for-input char) char)))
  (kill-region (point) (progn
                         (search-forward (char-to-string char)
                                         nil nil arg)
                         (point))))

;; (search-forward "TARGET-STRING"
;;                 LIMIT-OF-SEARCH
;;                 WHAT-TO-DO-IF-SEARCH-FAILS
;;                 REPEAT-COUNT)

;; ‘progn’ is a special form that causes each of its arguments to be
;; evaluated in sequence and then returns the value of the last one


;; 8.2 ‘kill-region’
(condition-case
    VAR
    BODYFORM
  ERROR-HANDLER...)

;; A ‘when’ expression is an ‘if’ without an else clause
;; The ‘unless’ macro is an ‘if’ without a then clause


;; 8.3 ‘copy-region-as-kill’


;; 8.4 Digression into C

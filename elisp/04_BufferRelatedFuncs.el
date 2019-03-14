;; 4.1 Finding More Information
;; ‘C-h f’ -> ‘describe-function’
;; ‘C-h v’ -> ‘describe-variable’
;; ‘M-.’   -> ‘xref-find-definitions’


;; 4.2 A Simplified ‘beginning-of-buffer’ Definition
(defun simplified-beginning-of-buffer ()
  "Move point to the beginning of the buffer;
     leave mark at previous position."
  (interactive)
  (push-mark)
  (goto-char (point-min)))


;; 4.3 The Definition of ‘mark-whole-buffer’
(defun mark-whole-buffer ()
  "Put point at beginning and mark at end of buffer.
     You probably should not use this function in Lisp programs;
     it is usually a mistake for a Lisp function to use any subroutine
     that uses or sets the mark."
  (interactive)
  (push-mark (point))
  (push-mark (point-max) nil t)
  (goto-char (point-min)))


;; 4.4 The Definition of ‘append-to-buffer’
(defun append-to-buffer (buffer start end)
  "Append to specified buffer the text of the region.
     It is inserted into that buffer before its point.

     When calling from a program, give three arguments:
     BUFFER (or buffer name), START and END.
     START and END specify the portion of the current buffer to be copied."
  (interactive
   (list (read-buffer "Append to buffer: " (other-buffer
                                            (current-buffer) t))
         (region-beginning) (region-end)))
  (let ((oldbuf (current-buffer)))
    (save-excursion
      (let* ((append-to (get-buffer-create buffer))
             (windows (get-buffer-window-list append-to t t))
             point)
        (set-buffer append-to)
        (setq point (point))
        (barf-if-buffer-read-only)
        (insert-buffer-substring oldbuf start end)
        (dolist (window windows)
          (when (= (window-point window) point)
            (set-window-point window (point))))))))


;; 4.6 Exercises
;; Write your own ‘simplified-end-of-buffer’ function definition; then
;; test it to see whether it works.
(defun simplified-end-of-buffer ()
  "go to end"
  (push-mark)
  (goto-char (point-max))
  )

(simplified-end-of-buffer)

;; Use ‘if’ and ‘get-buffer’ to write a function that prints a message
;; telling you whether a buffer exists.
(defun is-buf-exist (buf-name)
  "test if a buffer exist"
  (interactive "BBuffer Name: ")
  (if (get-buffer buf-name)
      (message "%s exists" buf-name)
    (message "%s does not exist" buf-name))
  )

(is-buf-exist "abc")
(is-buf-exist (get-buffer (current-buffer)))

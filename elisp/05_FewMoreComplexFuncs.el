;; 5.1 The Definition of ‘copy-to-buffer’
(defun copy-to-buffer (buffer start end)
  "Copy to specified buffer the text of the region.
It is inserted into that buffer, replacing existing text there.

When calling from a program, give three arguments:
BUFFER (or buffer name), START and END.
START and END specify the portion of the current buffer to be copied."
  (interactive "BCopy to buffer: \nr")
  (let ((oldbuf (current-buffer)))
    (with-current-buffer (get-buffer-create buffer)
      (barf-if-buffer-read-only)
      (erase-buffer)
      (save-excursion
        (insert-buffer-substring oldbuf start end)))))


;; 5.2 The Definition of ‘insert-buffer’
(defun insert-buffer (buffer)
  "Insert after point the contents of BUFFER.
     Puts mark after the inserted text.
     BUFFER may be a buffer or a buffer name."
  (interactive "*bInsert buffer: ")
  (or (bufferp buffer)
      (setq buffer (get-buffer buffer)))
  (let (start end newmark)
    (save-excursion
      (save-excursion
        (set-buffer buffer)
        (setq start (point-min) end (point-max)))
      (insert-buffer-substring buffer start end)
      (setq newmark (point)))
    (push-mark newmark)))

;; The asterisk in interactive is for the situation when the current buffer is a read-only
;; buffer. If ‘insert-buffer’ is called when the current buffer is read-only, a message to
;; this effect is printed in the echo area and the terminal may beep or blink at you;

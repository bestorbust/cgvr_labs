
import numpy as np
from OpenGL.GL import *
from OpenGL.GLUT import *
from OpenGL.GLU import *

WIDTH, HEIGHT = 1000, 700

# Clipping window boundaries
XMIN, YMIN = -200, -120
XMAX, YMAX = 200, 120

INSIDE = 0
LEFT = 1
RIGHT = 2
BOTTOM = 4
TOP = 8

lines = [
    (-300, 0, 300, 0),
    (-250, 180, 250, -180),
    (-100, -200, 100, 200),
    (-300, 180, -250, 220),
    (-150, 50, 150, 80),
    (0, -200, 0, 200)
]


# ---------------- REGION CODE ----------------

def compute_code(x, y):
    code = INSIDE

    if x < XMIN:
        code |= LEFT
    elif x > XMAX:
        code |= RIGHT

    if y < YMIN:
        code |= BOTTOM
    elif y > YMAX:
        code |= TOP

    return code


# ---------------- COHEN-SUTHERLAND ----------------

def cohen_sutherland_clip(x1, y1, x2, y2):
    code1 = compute_code(x1, y1)
    code2 = compute_code(x2, y2)

    while True:
        # Both endpoints are inside
        if not (code1 | code2):
            return (x1, y1, x2, y2)

        # Both endpoints share an outside region
        if code1 & code2:
            return None

        # Choose an endpoint outside the window
        code_out = code1 if code1 else code2

        if code_out & TOP:
            if y2 == y1:
                return None
            x = x1 + (x2 - x1) * (YMAX - y1) / (y2 - y1)
            y = YMAX

        elif code_out & BOTTOM:
            if y2 == y1:
                return None
            x = x1 + (x2 - x1) * (YMIN - y1) / (y2 - y1)
            y = YMIN

        elif code_out & RIGHT:
            if x2 == x1:
                return None
            y = y1 + (y2 - y1) * (XMAX - x1) / (x2 - x1)
            x = XMAX

        elif code_out & LEFT:
            if x2 == x1:
                return None
            y = y1 + (y2 - y1) * (XMIN - x1) / (x2 - x1)
            x = XMIN

        if code_out == code1:
            x1, y1 = x, y
            code1 = compute_code(x1, y1)
        else:
            x2, y2 = x, y
            code2 = compute_code(x2, y2)


# ---------------- DRAWING ----------------

def draw_text(x, y, text):
    glColor3f(1, 1, 1)
    glRasterPos2f(x, y)

    for ch in text:
        glutBitmapCharacter(GLUT_BITMAP_HELVETICA_18, ord(ch))


def draw_line(x1, y1, x2, y2, color, width=2):
    glColor3f(*color)
    glLineWidth(width)

    glBegin(GL_LINES)
    glVertex2f(x1, y1)
    glVertex2f(x2, y2)
    glEnd()


def draw_clipping_window():
    glColor3f(1.0, 0.85, 0.2)
    glLineWidth(3)

    glBegin(GL_LINE_LOOP)
    glVertex2f(XMIN, YMIN)
    glVertex2f(XMAX, YMIN)
    glVertex2f(XMAX, YMAX)
    glVertex2f(XMIN, YMAX)
    glEnd()


# ---------------- DISPLAY ----------------

def display():
    glClear(GL_COLOR_BUFFER_BIT)

    # Draw coordinate axes
    draw_line(-450, 0, 450, 0, (0.25, 0.3, 0.4), 1)
    draw_line(0, -280, 0, 280, (0.25, 0.3, 0.4), 1)

    # Draw clipping rectangle
    draw_clipping_window()

    # Original lines in red
    for x1, y1, x2, y2 in lines:
        draw_line(x1, y1, x2, y2, (0.9, 0.25, 0.3), 1)

    # Accepted clipped portions in green
    for line in lines:
        clipped = cohen_sutherland_clip(*line)

        if clipped is not None:
            draw_line(*clipped, (0.2, 1.0, 0.45), 4)

    draw_text(-450, 300, "COHEN-SUTHERLAND LINE CLIPPING")
    draw_text(-450, 270, "Yellow: Clipping Window")
    draw_text(-450, 245, "Red: Original Lines")
    draw_text(-450, 220, "Green: Accepted Clipped Portions")

    glFlush()


# ---------------- INITIALIZATION ----------------

def init():
    glClearColor(0.04, 0.05, 0.09, 1)

    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()
    gluOrtho2D(-500, 500, -350, 350)

    glMatrixMode(GL_MODELVIEW)
    glLoadIdentity()


# ---------------- MAIN ----------------

def main():
    glutInit()
    glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB)
    glutInitWindowSize(WIDTH, HEIGHT)
    glutCreateWindow(b"Lab 5 - Cohen-Sutherland Line Clipping")

    init()
    glutDisplayFunc(display)
    glutMainLoop()


if __name__ == "__main__":
    main()

import math
import numpy as np

from OpenGL.GL import *
from OpenGL.GLUT import *
from OpenGL.GLU import *


WIDTH = 1200
HEIGHT = 750


# =========================================================
# TRANSFORMATION MATRICES
# =========================================================

def translation(tx, ty):
    return np.array([
        [1, 0, tx],
        [0, 1, ty],
        [0, 0, 1]
    ], dtype=float)


def rotation(deg):
    a = math.radians(deg)
    c = math.cos(a)
    s = math.sin(a)

    return np.array([
        [c, -s, 0],
        [s,  c, 0],
        [0,  0, 1]
    ], dtype=float)


def scaling(sx, sy):
    return np.array([
        [sx, 0,  0],
        [0, sy, 0],
        [0,  0,  1]
    ], dtype=float)


def reflection_x():
    return np.array([
        [1,  0, 0],
        [0, -1, 0],
        [0,  0, 1]
    ], dtype=float)


def shearing(shx, shy):
    return np.array([
        [1,   shx, 0],
        [shy, 1,   0],
        [0,   0,   1]
    ], dtype=float)


# =========================================================
# APPLY HOMOGENEOUS TRANSFORMATION
# =========================================================

def apply(points, matrix):

    homogeneous = np.column_stack(
        (points, np.ones(len(points)))
    )

    transformed = (matrix @ homogeneous.T).T

    return transformed[:, :2]


# =========================================================
# DRAW TEXT
# =========================================================

def draw_text(x, y, text, size=GLUT_BITMAP_HELVETICA_18):

    glColor3f(1.0, 1.0, 1.0)

    glRasterPos2f(x, y)

    for character in text:
        glutBitmapCharacter(size, ord(character))


# =========================================================
# DRAW AXES
# =========================================================

def draw_axes(cx, cy):

    glColor3f(0.30, 0.32, 0.38)

    glLineWidth(1)

    glBegin(GL_LINES)

    # X-axis
    glVertex2f(cx - 170, cy)
    glVertex2f(cx + 170, cy)

    # Y-axis
    glVertex2f(cx, cy - 130)
    glVertex2f(cx, cy + 130)

    glEnd()


# =========================================================
# DRAW ARROW SHAPE
# =========================================================

def draw_shape(points, cx, cy, line_color):

    glColor3f(*line_color)

    glLineWidth(3)

    glBegin(GL_LINE_LOOP)

    for x, y in points:
        glVertex2f(cx + x, cy + y)

    glEnd()


# =========================================================
# DISPLAY
# =========================================================

def display():

    glClear(GL_COLOR_BUFFER_BIT)

    # -----------------------------------------------------
    # ORIGINAL ASYMMETRIC ARROW
    # -----------------------------------------------------

    shape = np.array([
        [-90, -35],
        [ 10, -35],
        [ 10, -65],
        [ 90,   0],
        [ 10,  65],
        [ 10,  35],
        [-90,  35]
    ], dtype=float)


    # -----------------------------------------------------
    # TRANSFORMATIONS
    # -----------------------------------------------------

    variants = [

        (
            "ORIGINAL",
            shape,
            (1.0, 1.0, 1.0)
        ),

        (
            "TRANSLATION",
            apply(
                shape,
                translation(55, 35)
            ),
            (0.0, 0.9, 1.0)
        ),

        (
            "ROTATION 40 deg",
            apply(
                shape,
                rotation(40)
            ),
            (1.0, 0.3, 0.4)
        ),

        (
            "SCALING 1.4 x 0.65",
            apply(
                shape,
                scaling(1.4, 0.65)
            ),
            (0.2, 1.0, 0.5)
        ),

        (
            "REFLECTION ABOUT X",
            apply(
                shape,
                reflection_x()
            ),
            (1.0, 0.8, 0.1)
        ),

        (
            "SHEARING X = 0.7",
            apply(
                shape,
                shearing(0.7, 0)
            ),
            (0.7, 0.4, 1.0)
        )
    ]


    # -----------------------------------------------------
    # PANEL POSITIONS
    # -----------------------------------------------------

    positions = [

        (200, 500),
        (600, 500),
        (1000, 500),

        (200, 190),
        (600, 190),
        (1000, 190)
    ]


    # -----------------------------------------------------
    # DRAW ALL PANELS
    # -----------------------------------------------------

    for (title, points, color), (cx, cy) in zip(
        variants,
        positions
    ):

        # Panel border

        glColor3f(
            0.25,
            0.28,
            0.35
        )

        glLineWidth(2)

        glBegin(GL_LINE_LOOP)

        glVertex2f(cx - 180, cy - 140)
        glVertex2f(cx + 180, cy - 140)
        glVertex2f(cx + 180, cy + 140)
        glVertex2f(cx - 180, cy + 140)

        glEnd()


        # Axes

        draw_axes(cx, cy)


        # Shape

        draw_shape(
            points,
            cx,
            cy,
            color
        )


        # Label

        draw_text(
            cx - 155,
            cy + 115,
            title
        )


    # -----------------------------------------------------
    # MAIN TITLE
    # -----------------------------------------------------

    draw_text(
        340,
        705,
        "LAB 3 - 2D TRANSFORMATIONS",
        GLUT_BITMAP_HELVETICA_18
    )




    glFlush()


# =========================================================
# INITIALIZATION
# =========================================================

def init():

    glClearColor(
        0.05,
        0.06,
        0.10,
        1.0
    )

    glMatrixMode(GL_PROJECTION)

    glLoadIdentity()

    gluOrtho2D(
        0,
        WIDTH,
        0,
        HEIGHT
    )

    glMatrixMode(GL_MODELVIEW)

    glLoadIdentity()


# =========================================================
# MAIN
# =========================================================

def main():

    glutInit()

    glutInitDisplayMode(
        GLUT_SINGLE | GLUT_RGB
    )

    glutInitWindowSize(
        WIDTH,
        HEIGHT
    )

    glutCreateWindow(
        b"Lab 3 - 2D Transformations"
    )

    init()

    glutDisplayFunc(display)

    glutMainLoop()


if __name__ == "__main__":
    main()
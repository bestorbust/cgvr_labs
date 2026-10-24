
import math
import numpy as np

from OpenGL.GL import *
from OpenGL.GLUT import *
from OpenGL.GLU import *

WIDTH, HEIGHT = 1100, 720


# ---------------- TRANSFORMATION MATRICES ----------------

def T(tx, ty):
    return np.array([
        [1, 0, tx],
        [0, 1, ty],
        [0, 0, 1]
    ], float)


def R(deg):
    a = math.radians(deg)
    c, s = math.cos(a), math.sin(a)

    return np.array([
        [c, -s, 0],
        [s,  c, 0],
        [0,  0, 1]
    ], float)


def S(sx, sy):
    return np.array([
        [sx,  0, 0],
        [0, sy, 0],
        [0,  0, 1]
    ], float)


# ---------------- APPLY TRANSFORMATION ----------------

def transform(points, matrix):
    hom = np.column_stack((points, np.ones(len(points))))
    return (matrix @ hom.T).T[:, :2]


def to_screen(points):
    ox, oy = WIDTH // 2, HEIGHT // 2 + 30
    return [(int(ox + x), int(oy - y)) for x, y in points]


# ---------------- DRAWING FUNCTIONS ----------------

def draw_text(x, y, text, font=GLUT_BITMAP_HELVETICA_12):
    glColor3f(0.92, 0.94, 0.98)
    glRasterPos2f(x, y)

    for ch in text:
        glutBitmapCharacter(font, ord(ch))


def draw_axes(cx, cy):
    glColor3f(0.28, 0.32, 0.40)
    glLineWidth(1)

    glBegin(GL_LINES)
    glVertex2f(cx - 145, cy)
    glVertex2f(cx + 145, cy)
    glVertex2f(cx, cy - 100)
    glVertex2f(cx, cy + 100)
    glEnd()


def draw_shape(points, color):
    screen_points = to_screen(points)

    glColor3f(*color)
    glLineWidth(3)

    glBegin(GL_LINE_LOOP)
    for x, y in screen_points:
        glVertex2f(x, HEIGHT - y)
    glEnd()


def draw_panel(cx, cy, title, description, points, color):
    glColor3f(0.25, 0.29, 0.37)

    glBegin(GL_LINE_LOOP)
    glVertex2f(cx - 165, cy - 135)
    glVertex2f(cx + 165, cy - 135)
    glVertex2f(cx + 165, cy + 135)
    glVertex2f(cx - 165, cy + 135)
    glEnd()

    draw_text(cx - 150, cy + 110, title)
    draw_text(cx - 150, cy + 88, description)

    draw_axes(cx, cy)

    # Draw directly in the panel's coordinate system.
    glColor3f(*color)
    glLineWidth(3)

    glBegin(GL_LINE_LOOP)
    for x, y in points:
        glVertex2f(cx + x, cy + y)
    glEnd()


# ---------------- DISPLAY ----------------

def display():
    glClear(GL_COLOR_BUFFER_BIT)

    # Asymmetric arrow shape
    shape = np.array([
        [-65, -30],
        [ 20, -30],
        [ 20, -55],
        [ 85,   0],
        [ 20,  55],
        [ 20,  30],
        [-65,  30]
    ], float)

    # Individual transformation matrices
    translation = T(35, 20)
    rotation = R(35)
    scaling = S(1.25, 0.75)

    # Composite transformations.
    # With column vectors, the rightmost matrix acts first.
    TR = translation @ rotation
    RT = rotation @ translation
    STR = scaling @ translation @ rotation

    variants = [
        (
            "ORIGINAL",
            "No transformation",
            shape,
            (0.95, 0.95, 0.98)
        ),
        (
            "T @ R",
            "Rotate, then translate",
            transform(shape, TR),
            (0.1, 0.85, 1.0)
        ),
        (
            "R @ T",
            "Translate, then rotate",
            transform(shape, RT),
            (1.0, 0.4, 0.45)
        ),
        (
            "S @ T @ R",
            "Rotate, translate, scale",
            transform(shape, STR),
            (0.3, 1.0, 0.55)
        )
    ]

    positions = [
        (285, 480),
        (815, 480),
        (285, 190),
        (815, 190)
    ]

    draw_text(
        350, 680,
        "LAB 4 - COMPOSITE TRANSFORMATIONS",
        GLUT_BITMAP_HELVETICA_18
    )
    draw_text(
        350, 650,
        "Homogeneous Coordinates | Transformation Order"
    )

    for item, (cx, cy) in zip(variants, positions):
        title, desc, points, color = item
        draw_panel(cx, cy, title, desc, points, color)

    glFlush()


# ---------------- INITIALIZATION ----------------

def init():
    glClearColor(0.05, 0.06, 0.10, 1.0)

    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()
    gluOrtho2D(0, WIDTH, 0, HEIGHT)

    glMatrixMode(GL_MODELVIEW)
    glLoadIdentity()


# ---------------- MAIN ----------------

def main():
    glutInit()
    glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB)
    glutInitWindowSize(WIDTH, HEIGHT)

    glutCreateWindow(b"Lab 4 - Composite Transformations")

    init()
    glutDisplayFunc(display)
    glutMainLoop()


if __name__ == "__main__":
    main()


from OpenGL.GL import *
from OpenGL.GLUT import *
from OpenGL.GLU import *

WIDTH, HEIGHT = 1100, 650


# ---------------- DRAW 2D SHAPES ----------------

def draw_rectangle(x1, y1, x2, y2, z, color):
    glColor3f(*color)

    glBegin(GL_QUADS)
    glVertex3f(x1, y1, z)
    glVertex3f(x2, y1, z)
    glVertex3f(x2, y2, z)
    glVertex3f(x1, y2, z)
    glEnd()


def draw_triangle(points, z, color):
    glColor3f(*color)

    glBegin(GL_TRIANGLES)
    for x, y in points:
        glVertex3f(x, y, z)
    glEnd()


# ---------------- SCENE ----------------

def draw_scene():
    # Draw order is identical in both panels.
    # Smaller Z values are closer to the camera.

    # Shape 1: large blue rectangle, farthest away
    draw_rectangle(
        -0.85, -0.65, 0.85, 0.65,
        0.7, (0.1, 0.4, 1.0)
    )

    # Shape 2: green triangle, at middle depth
    draw_triangle(
        [(-0.65, -0.45), (0.65, -0.45), (0.0, 0.75)],
        0.0, (0.1, 0.9, 0.35)
    )

    # Shape 3: red rectangle, closest to the camera.
    # It is intentionally drawn LAST.
    draw_rectangle(
        -0.35, -0.30, 0.75, 0.30,
        -0.5, (1.0, 0.2, 0.2)
    )


# ---------------- TEXT ----------------

def draw_text(x, y, text):
    glDisable(GL_DEPTH_TEST)

    glMatrixMode(GL_PROJECTION)
    glPushMatrix()
    glLoadIdentity()
    gluOrtho2D(0, WIDTH, 0, HEIGHT)

    glMatrixMode(GL_MODELVIEW)
    glPushMatrix()
    glLoadIdentity()

    glColor3f(0.95, 0.97, 1.0)
    glRasterPos2f(x, y)

    for ch in text:
        glutBitmapCharacter(GLUT_BITMAP_HELVETICA_18, ord(ch))

    glPopMatrix()

    glMatrixMode(GL_PROJECTION)
    glPopMatrix()

    glMatrixMode(GL_MODELVIEW)
    glEnable(GL_DEPTH_TEST)


# ---------------- PANEL RENDERING ----------------

def render_panel(x, width, use_zbuffer):
    glViewport(x, 0, width, HEIGHT)

    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()
    gluOrtho2D(-1.2, 1.2, -1.0, 1.0)

    glMatrixMode(GL_MODELVIEW)
    glLoadIdentity()

    if use_zbuffer:
        glEnable(GL_DEPTH_TEST)
        glDepthFunc(GL_LESS)
    else:
        glDisable(GL_DEPTH_TEST)

    glEnable(GL_SCISSOR_TEST)
    glScissor(x, 0, width, HEIGHT)
    glClearColor(0.04, 0.05, 0.09, 1.0)
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
    glDisable(GL_SCISSOR_TEST)

    draw_scene()


# ---------------- DISPLAY ----------------

def display():
    half = WIDTH // 2

    # Left: no depth testing
    render_panel(0, half, False)

    # Right: depth testing enabled
    render_panel(half, WIDTH - half, True)

    # Labels in screen coordinates
    glViewport(0, 0, WIDTH, HEIGHT)

    draw_text(25, HEIGHT - 40, "WITHOUT Z-BUFFER")
    draw_text(half + 25, HEIGHT - 40, "WITH Z-BUFFER")

    draw_text(25, 30, "Draw order determines visibility")
    draw_text(half + 25, 30, "Depth determines visibility")

    # Vertical divider
    glDisable(GL_DEPTH_TEST)
    glColor3f(0.5, 0.55, 0.65)

    glBegin(GL_LINES)
    glVertex2f(half, 0)
    glVertex2f(half, HEIGHT)
    glEnd()

    glutSwapBuffers()


# ---------------- INITIALIZATION ----------------

def init():
    glClearColor(0.04, 0.05, 0.09, 1.0)


# ---------------- MAIN ----------------

def main():
    glutInit()
    glutInitDisplayMode(GLUT_DOUBLE | GLUT_RGB | GLUT_DEPTH)
    glutInitWindowSize(WIDTH, HEIGHT)

    glutCreateWindow(b"Lab 6 - Z-Buffer Hidden Surface Removal")

    init()
    glutDisplayFunc(display)
    glutMainLoop()


if __name__ == "__main__":
    main()

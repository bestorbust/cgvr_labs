from OpenGL.GL import *
from OpenGL.GLUT import *
from OpenGL.GLU import *

points = []


def midPoint_circle(r):
    points.clear()

    x = 0
    y = r
    p = 1 - r

    while x <= y:

        points.extend([
            (x, y),
            (y, x),
            (-x, y),
            (-y, x),
            (x, -y),
            (y, -x),
            (-x, -y),
            (-y, -x)
        ])

        if p < 0:
            x += 1
            p += 2 * x + 1
        else:
            x += 1
            y -= 1
            p += 2 * x - 2 * y + 1

    return points


def display():

    glClear(GL_COLOR_BUFFER_BIT)

    glColor3f(0, 1, 0)
    glPointSize(5)

    glBegin(GL_POINTS)

    for x, y in points:
        glVertex2f(x + 100, y + 100)

    glEnd()

    glFlush()


def init():

    glClearColor(0, 0, 0, 1)

    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()

    gluOrtho2D(0, 200, 0, 200)


points = midPoint_circle(50)

glutInit()
glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB)
glutInitWindowSize(800, 600)
glutCreateWindow(b"Midpoint Circle Algorithm")

init()

glutDisplayFunc(display)

glutMainLoop()
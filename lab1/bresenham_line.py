from OpenGL.GL import *
from OpenGL.GLUT import *
from OpenGL.GLU import *

points = []

def bresenham_line(start_x, start_y, end_x, end_y):

    x, y = start_x, start_y

    delta_x = abs(end_x - start_x)
    delta_y = abs(end_y - start_y)

    points.clear()
    points.append((x, y))

    slope = delta_y / delta_x

    if 0 <= slope <= 1:

        p = [0] * (delta_x + 1)

        parameter1 = 2 * delta_y
        parameter2 = 2 * delta_y - 2 * delta_x

        p[0] = parameter1 - delta_x

        for i in range(delta_x):

            if p[i] < 0:
                x = x + 1
                p[i + 1] = p[i] + parameter1

            else:
                x, y = x + 1, y + 1
                p[i + 1] = p[i] + parameter2

            points.append((x, y))

    if slope > 1:

        p = [0] * (delta_y + 1)

        parameter1 = 2 * delta_x
        parameter2 = 2 * delta_x - 2 * delta_y

        p[0] = parameter1 - delta_y

        for i in range(delta_y):

            if p[i] < 0:
                y = y + 1
                p[i + 1] = p[i] + parameter1

            else:
                x, y = x + 1, y + 1
                p[i + 1] = p[i] + parameter2

            points.append((x, y))


def display():

    glClear(GL_COLOR_BUFFER_BIT)

    glColor3f(0.0, 1.0, 0.0)
    glPointSize(5)

    glBegin(GL_POINTS)

    for x, y in points:
        glVertex2f(x, y)

    glEnd()

    glFlush()


def init():

    glClearColor(0.0, 0.0, 0.0, 1.0)

    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()

    gluOrtho2D(0, 800, 0, 600)


bresenham_line(100, 100, 700, 500)


glutInit()
glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB)

glutInitWindowSize(800, 600)
glutInitWindowPosition(100, 100)

glutCreateWindow(b"Bresenham Line Algorithm")

init()

glutDisplayFunc(display)

glutMainLoop()